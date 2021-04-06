﻿using System;
using Bamboozed.Domain.TimeOffRequest;
using CSharpFunctionalExtensions;

namespace Bamboozed.Domain.TimeOffPolicy
{
    public abstract class UserTimeOffPolicy: Entity<string>
    {
        public string UserEmail { get; protected set; }
        public TimeOffAction Action { get; protected set; }
        public TimeOffType TimeOffType { get; protected set; }

        protected  UserTimeOffPolicy() { }

        protected UserTimeOffPolicy(string userEmail, TimeOffAction action, TimeOffType timeOffType)
            : base(Guid.NewGuid().ToString())
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                throw new ArgumentNullException($"{userEmail} cannot be empty");
            }

            UserEmail = userEmail;
            Action = action;
            TimeOffType = timeOffType;
        }
    }
}