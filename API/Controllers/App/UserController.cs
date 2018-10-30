using Agrishare.API.Models;
using Agrishare.Core;
using System;
using System.Text.RegularExpressions;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
{
    public class UserController : BaseApiController
    {
        private const int MaxFailedLoginAttempts = 5;

        [Route("register/telephone/lookup")]
        [AcceptVerbs("GET")]
        public object FindTelephone(string Telephone)
        {
            var user = Entities.User.Find(Telephone: Telephone);            
            if (user?.Id > 0 && !user.Telephone.IsEmpty())
                return Success(new
                {
                    User = new
                    {
                        user.Id,
                        user.FirstName,
                        user.StatusId
                    }
                });

            return Error("Please register to continue");
        }

        [Route("register")]
        [AcceptVerbs("POST")]
        public object Register(UserModel User)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            var user = new Entities.User
            {
                FirstName = User.FirstName,
                LastName = User.LastName,
                Telephone = User.Telephone,
                EmailAddress = User.EmailAddress,
                DateOfBirth = User.DateOfBirth,
                GenderId = User.GenderId,
                ClearPassword = User.PIN
            };

            if (!Regex.IsMatch(user.Telephone, @"^07[\d]{8}"))
                return Error($"{user.Telephone} is not a valid cell number. The number should start with 07 and contain 10 digits.");

            if (!user.UniqueTelephone())
                return Error($"{user.Telephone} has already been registered");

            if (!user.EmailAddress.IsEmpty() && !user.UniqueEmailAddress())
                return Error($"{user.EmailAddress} has already been registered");

            user.RoleList = $"{Entities.Role.User}";
            user.NotificationPreferences = (int)Entities.NotificationPreferences.PushNotifications + (int)Entities.NotificationPreferences.SMS;
            user.StatusId = Entities.UserStatus.Pending;
            if (user.Save())
            {
                Entities.Counter.Hit(CurrentUser.Id, Entities.Counters.Register);

                if (!user.SendVerificationCode())
                {
                    user.Delete();
                    return Error("Unable to send verification code - please try again");
                }

                return Success(new
                {
                    User = user.ProfileJson()
                });
            }

            return Error("An unknown error occurred.");
        }

        [Route("register/code/verify")]
        [AcceptVerbs("GET")]
        public object VerifyCode(int UserId, string Code)
        {
            var user = Entities.User.Find(Id: UserId);

            if (user?.FailedLoginAttempts > MaxFailedLoginAttempts)
                return Error("Your account has been locked - please reset your PIN.");

            if (user?.VerificationCode == Code)
            {
                if (user.VerificationCodeExpiry < DateTime.UtcNow)
                    return Error("This code has expired");

                user.VerificationCode = string.Empty;
                user.StatusId = Entities.UserStatus.Verified;
                user.AuthToken = Guid.NewGuid().ToString();
                user.Save();

                return new
                {
                    User = user.ProfileJson()
                };
            }

            if (user.Id > 0)
            {
                user.FailedLoginAttempts += 1;
                user.Save();
            }

            return Error("Invalid code");
        }

        [Route("login")]
        [AcceptVerbs("GET")]
        public object Login(string Telephone, string PIN)
        {
            var user = Entities.User.Find(Telephone: Telephone);

            if (user == null || user?.Id == 0)
                return Error("Phone number or PIN not recognised.");

            if (user?.StatusId == Entities.UserStatus.Pending)
                return Error("Your account has not been verified - please reset your PIN.");

            if (user.FailedLoginAttempts > MaxFailedLoginAttempts)
                return Error("Your account has been locked - please reset your PIN.");

            if (user.ValidatePassword(PIN))
            {
                user.FailedLoginAttempts = 0;
                if (user.AuthToken.IsEmpty())
                    user.AuthToken = Guid.NewGuid().ToString();
                user.Save();

                Entities.Counter.Hit(CurrentUser.Id, Entities.Counters.Login);

                return Success(new
                {
                    Auth = new
                    {
                        User = user.ProfileJson()
                    }
                });
            }

            if (user.Id > 0)
            {
                user.FailedLoginAttempts += 1;
                user.Save();
            }

            return Error("Phone number or PIN not recognised.");
        }

        [Route("pin/change")]
        [AcceptVerbs("GET")]
        public object ChangePin(string Telephone, string Code, string PIN)
        {
            var user = Entities.User.Find(Telephone: Telephone);

            if (user == null || user?.Id == 0)
                return Error("Invalid code");

            if (user.VerificationCode != Code)
                return Error("Invalid code");

            user.ClearPassword = PIN;
            if (user.Save())
            {
                user.FailedLoginAttempts = 0;
                if (user.AuthToken.IsEmpty())
                    user.AuthToken = Guid.NewGuid().ToString();
                user.Save();

                Entities.Counter.Hit(CurrentUser.Id, Entities.Counters.ResetPIN);

                return Success(new
                {
                    User = user.ProfileJson()
                });
            }

            return Error("An unknown error occurred");
        }

        [Route("code/resend")]
        [AcceptVerbs("GET")]
        public object SendCode(string Telephone)
        {
            var user = Entities.User.Find(Telephone: Telephone);
            if (user?.Id > 0 && user.SendVerificationCode())
            {
                Entities.Counter.Hit(CurrentUser.Id, Entities.Counters.ForgotPIN);
                return Success("Please check your messages");
            }

            return Error("Unable to send verification code - please try again");
        }

        [@Authorize(Roles = "User")]
        [Route("profile/update")]
        [AcceptVerbs("POST")]
        public object UpdateProfile(UserModel User)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            CurrentUser.FirstName = User.FirstName;
            CurrentUser.LastName = User.LastName;
            CurrentUser.Telephone = User.Telephone;
            CurrentUser.EmailAddress = User.EmailAddress;
            CurrentUser.DateOfBirth = User.DateOfBirth;
            CurrentUser.GenderId = User.GenderId;

            if (!Regex.IsMatch(CurrentUser.Telephone, @"^07[\d]{8}"))
                return Error($"{CurrentUser.Telephone} is not a valid cell number. The number should start with 07 and contain 10 digits.");

            if (!CurrentUser.UniqueTelephone())
                return Error($"{CurrentUser.Telephone} has already been registered");

            if (!CurrentUser.EmailAddress.IsEmpty() && !CurrentUser.UniqueEmailAddress())
                return Error($"{CurrentUser.EmailAddress} has already been registered");

            if (CurrentUser.Save())
                return Success(new
                {
                    User = CurrentUser.ProfileJson()
                });

            return Error("An unknown error occurred");
        }

        [@Authorize(Roles = "User")]
        [Route("profile/preferences/notifications/update")]
        [AcceptVerbs("GET")]
        public object UpdateNotificationPreferences(bool SMS, bool PushNotifications, bool Email)
        {
            CurrentUser.NotificationPreferences = 0;
            if (SMS)
                CurrentUser.NotificationPreferences += (int)Entities.NotificationPreferences.SMS;
            if (PushNotifications)
                CurrentUser.NotificationPreferences += (int)Entities.NotificationPreferences.PushNotifications;
            if (Email)
                CurrentUser.NotificationPreferences += (int)Entities.NotificationPreferences.Email;

            if (CurrentUser.Save())
                return Success(new
                {
                    User = CurrentUser.ProfileJson()
                });

            return Error("Could not update preferences");
        }

        [@Authorize(Roles = "User")]
        [Route("me")]
        [AcceptVerbs("GET")]
        public object Me()
        {
            return Success(new
            {
                User = CurrentUser.ProfileJson()
            });
        }

        [@Authorize(Roles = "User")]
        [Route("device/register")]
        [AcceptVerbs("GET")]
        public object RegisterDevice(string Token)
        {
            var device = Entities.Device.Find(Token: Token);

            if (device == null)
                device = new Entities.Device
                {
                    Token = Token,
                    User = CurrentUser
                };

            if (device.Save())
                return Success();

            return Error("Could not register device");
        }

        [Route("logout")]
        [AcceptVerbs("GET")]
        public object Logout(string Token)
        {
            var device = Entities.Device.Find(Token: Token);
            if (device?.Delete() ?? false)
                return Success();

            return Error("Device not found");
        }

        [Route("delete")]
        [AcceptVerbs("GET")]
        public object Delete()
        {
            var bookings = Entities.Booking.Count(UserId: CurrentUser.Id, StartDate: DateTime.Now, Upcoming: true);
            if (bookings > 0)
                return Error("Can not delete account - you have upcoming bookings");

            var listings = Entities.Listing.List(UserId: CurrentUser.Id);
            foreach(var listing in listings)
            {
                bookings = Entities.Booking.Count(ListingId: listing.Id, StartDate: DateTime.Now, Upcoming: true);
                if (bookings > 0)
                    return Error("Can not delete account - you have upcoming bookings");
            }
            foreach (var listing in listings)
                listing.Delete();

            var devices = Entities.Device.List(UserId: CurrentUser.Id);
            foreach(var device in devices)
                device.Delete();            

            CurrentUser.Delete();

            return Success("OK");
        }
    }
}
