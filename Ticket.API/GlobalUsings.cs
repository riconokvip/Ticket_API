/* --- Domain using --- */
global using Ticket.Domain.Entities;
global using Ticket.Domain.Enums;
global using Ticket.Domain.DbContexts;
global using Ticket.Domain.Extensions;
global using Ticket.Domain.Repositories;
global using Microsoft.EntityFrameworkCore;

/* --- Internal using --- */
// Base
global using Microsoft.AspNetCore.Mvc;
global using System.Text.RegularExpressions;
global using System.Text.Json;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel;
global using FastEnumUtility;
global using AutoMapper;
// Extensions
global using Ticket.API.Extensions;
// Middlewares
global using Ticket.API.Middlewares;
// JwtAuthentications
global using Ticket.API.JwtAuthentications;
// Models
global using Ticket.API.Models;
global using Ticket.API.Models.Auths;
// Services
global using Ticket.API.Services;