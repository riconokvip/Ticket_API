/* --- Domain using --- */
global using Ticket.Domain.Entities;
global using Ticket.Domain.Enums;
global using Ticket.Domain.DbContexts;
global using Ticket.Domain.Extensions;
global using Ticket.Domain.Repositories;
global using Microsoft.EntityFrameworkCore;

/* --- Infrastructure --- */
global using Ticket.Persistence;

/* --- Internal using --- */
// Base
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Authorization;
global using System.Text.RegularExpressions;
global using System.Text.Json;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel;
global using FastEnumUtility;
global using AutoMapper;
global using System.Security.Claims;
// Extensions
global using Ticket.API.Extensions;
// Middlewares
global using Ticket.API.Middlewares;
// JwtAuthentications
global using Ticket.API.JwtAuthentications;
// Models
global using Ticket.API.Models;
global using Ticket.API.Models.Auths;
global using Ticket.API.Models.WorkSpaces;
global using Ticket.API.Models.Projects;
global using Ticket.API.Models.WorkSpaceMembers;
global using Ticket.API.Models.ProjectMembers;
global using Ticket.API.Models.Tickets;
global using Ticket.API.Models.Users;
// Es Models
global using Ticket.API.EsModels;
// Services
global using Ticket.API.Services;
// Custom Authorizations
global using Ticket.API.Authorizations;