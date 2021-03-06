﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment2.Data;
using Assignment2.Models;
using SimpleHashing;
using X.PagedList;
using Microsoft.AspNetCore.Http;
using Assignment2.Attributes;
using Assignment2.ViewModels;
using Assignment2.Models.Builder;
using Assignment2.Controllers.Functions;

namespace Assignment2.Controllers
{
    [AuthorizeCustomer]
    public class ProfileController : Controller
    {
        private readonly MainContext _context;
        private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

        public ProfileController(MainContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ViewMyStatement(int? page = 1, AccountType accountType = AccountType.Saving)
        {
            // Retrieve customer object from context
            var customer = await _context.Customers.FindAsync(CustomerID);
            List<Account> accounts = customer.Accounts;
            ViewBag.Customer = customer;

            // Page the orders, maximum of 4 per page.
            const int pageSize = 4;

            ViewMyStatementVM viewModel = ViewMyStatementMediator.GenerateMyStatementViewModel(customer, accountType, (int)page, pageSize);

            return View(viewModel);
        }

        // GET: Profile/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                id = CustomerID;
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerID == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Profile/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewData["State"] = new SelectList(Enum.GetValues(typeof(AustralianState)));
            return View(customer);
        }

        // POST: Profile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerID,Name,TFN,Address,City,State,PostCode,Phone")] Customer customer)
        {
            if (id != customer.CustomerID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details));
            }
            ViewData["State"] = new SelectList(Enum.GetValues(typeof(AustralianState)));
            return View(customer);
        }

        public IActionResult ChangePassword(int? customerID) => View(_context.Logins.Where(x => x.CustomerID == customerID).FirstOrDefault());

        [HttpPost]
        public async Task<IActionResult> ChangePassword(int? customerID, string newPassword)
        {
            Login login = await _context.Logins.Where(x => x.CustomerID == customerID).FirstOrDefaultAsync();

            // Check if not old password
            if (PBKDF2.Verify(login.PasswordHash, newPassword))
            {
                ModelState.AddModelError(nameof(login.PasswordHash), "New password cannot be the same the old password");
                return View(login);
            }

            login.PasswordHash = PBKDF2.Hash(newPassword);
            login.ModifyDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), login.CustomerID);
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerID == id);
        }
    }
}
