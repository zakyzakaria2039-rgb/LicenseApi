﻿using LicenseApi.Data;
using LicenseApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LicenseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LicenseController : ControllerBase
    {
        private readonly LicenseDbContext _context;

        public LicenseController(LicenseDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterDevice([FromBody] DeviceInfo device)
        {
            if (string.IsNullOrWhiteSpace(device.UserId) || string.IsNullOrWhiteSpace(device.DeviceId))
                return BadRequest("Invalid userId or deviceId.");

            var existingDevices = await _context.Devices
                .Where(d => d.UserId == device.UserId)
                .ToListAsync();

            if (existingDevices.Any(d => d.DeviceId == device.DeviceId))
                return Ok("Device already registered.");

            if (existingDevices.Count >= 1)
                return BadRequest("Lisensi ini sedang di pakai di perangkat lain.");

            device.RegisteredAt = DateTime.UtcNow;
            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            return Ok("Device registered successfully.");
        }

        [HttpGet("devices")]
        public async Task<IActionResult> GetDeviceCount([FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("Missing userId.");

            var count = await _context.Devices
                .CountAsync(d => d.UserId == userId);

            return Ok(new { userId, count });
        }

        [HttpGet("device")]
        public async Task<IActionResult> GetDeviceInfo(string userId, string deviceId)
        {
            var device = await _context.Devices
                .FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceId == deviceId);

            if (device == null)
                return NotFound();

            return Ok(device);
        }

        [HttpDelete("delete")]
        public IActionResult DeleteDevice(string userId, string deviceId)
        {
            var device = _context.Devices.FirstOrDefault(d => d.UserId == userId && d.DeviceId == deviceId);
            if (device == null) return NotFound();

            _context.Devices.Remove(device);
            _context.SaveChanges();
            return Ok();
        }
    }
}

