﻿using AdminApi.Data;
using AdminApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminApi.Utils;
using Microsoft.EntityFrameworkCore;
using AdminApi.Models.Adapter;
using Microsoft.Extensions.DependencyInjection;

namespace AdminApi.Persistence
{
    public static class MainPersistence
    {
        // Moe timer variable outside to avoid session destroying for auto optimazation
        static System.Threading.Timer timer;
        public static void RunBillPayPersistence(IServiceProvider service)
        {
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromMinutes(1);

            // Will be executed every 1 minute
            timer = new System.Threading.Timer((e) =>
            {
                ExecuteBillPaySchedule(service);
            }, null, startTimeSpan, periodTimeSpan);
        }

        private static void ExecuteBillPaySchedule(IServiceProvider service)
        {
            var context = new MainContext(service.GetRequiredService<DbContextOptions<MainContext>>());

            BillPaysAdapter billPayAdapter = new BillPaysAdapter(context.BillPays.ToList(), context);
            billPayAdapter.ExecuteBillPaySchedule();

            context.SaveChangesAsync();
        }
    }
}
