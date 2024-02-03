﻿using Dropify.Models;
using System.Diagnostics.Eventing.Reader;
using System.Text;

namespace Dropify.Logics
{
    public class UserDAO
    {
        
        public List<User> GetAllUsers()
        {
            using (var db = new prn211_dropshippingContext())
            {
                return db.Users.ToList();
            }
        }
        public UserDetail Login(String email, String password)
        {
            using (var db = new prn211_dropshippingContext())
            {
                var user = db.Users.Where(u => u.Email == email && u.Pword == password).FirstOrDefault();
                if (user != null)
                {
                    return db.UserDetails.Where(u => u.Uid == user.Uid).FirstOrDefault();
                }
                return null;
            }
        }

        
        public bool Register(string email, string password, string? fullname)
        {
            using (var db = new prn211_dropshippingContext())
            {
                UserDAO userDAO = new UserDAO(); 
                var existingUser = db.Users.FirstOrDefault(u => u.Email == email);

                if (existingUser != null)
                {
                    return false;
                }
                else
                {
                    var newUser = new User
                    {
                        Email = email,
                        Pword = userDAO.Encryption(password)

                    };
                    db.Users.Add(newUser);
                    db.SaveChanges();
                    return true;
                }
            }
        }
        public bool Authentication(string email, string password)
        {
            using (var db = new prn211_dropshippingContext())
            {
                UserDAO userDAO = new UserDAO();
                var existingUser = db.Users.FirstOrDefault(u => u.Email == email);

                if (existingUser != null)
                {
                    string realpass = userDAO.DecryptPass(existingUser.Pword);
                    if(password.Equals(realpass)) { 
                        return true;
                    }
                }
                return false;
            }
        }
        private string Encryption(string password) 
        {
            byte[] storePassword = ASCIIEncoding.ASCII.GetBytes(password);
            string encryptpass = Convert.ToBase64String(storePassword);
            return encryptpass;
        }
        private string DecryptPass(string password) 
        {
            byte[] encryptpass = Convert.FromBase64String(password);
            string decryptpass = ASCIIEncoding.ASCII.GetString(encryptpass);
            return decryptpass;
        }
    }
}
