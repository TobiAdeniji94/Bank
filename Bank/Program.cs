using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.ComponentModel.Design;

namespace Bank {
    public class CreateAccount {
        private string[] user = new string[6];
        private string user_file = "account.txt";

        public void SetAccount() {
            Console.WriteLine("Welcome\nHere you will create your Bank account\nPlease fill out the form\n\n");
            string[] field = new string[6] { "first name:", "last name:", "DOB:", "password:", "account type:", "Initial deposit must be >=500\n$:" };

            int controller = 0;
            int max_try = 3;
            for (controller = 0; controller < 6; controller++) {
                Console.Write("{0}", field[controller]);
                user[controller] = Console.ReadLine();
            }

            // Parse the user input to an int and check if it is less than 500
            int deposit = 0;
            bool isNumber = int.TryParse(user[5], out deposit);
            if (!isNumber || deposit < 500) {
                Console.WriteLine("Sorry, enter a deposit >= 500");
                user[5] = "0.00";

                while (!isNumber || deposit < 500 && max_try > 0) {
                    Console.Write("{0}", field[5]);
                    user[5] = Console.ReadLine();
                    isNumber = int.TryParse(user[5], out deposit);
                    max_try--;
                }

                if (max_try <= 0 && !isNumber) {
                    Console.WriteLine("Sorry, your account could not be created\nTry again later!");
                    Console.ReadKey();
                    return;
                }
            }

            // Save the user account details to file
            try {
                File.WriteAllLines(user_file, user);
                Console.WriteLine("Congratulations!!\nYour account has been created successfully");
                Console.WriteLine("\n\n");
                
            }
            catch (Exception error) {
                Console.WriteLine(error.Message);
            }
        }
    }

    public class Account {
        private string[] user_info;
        private string[] options = new string[]{"1-> Account Enquiry","2-> Deposit funds","3-> Withdraw funds","4-> Change password"};
        private int opt_view = 0;
        private int limit = 1200;
        
        public Account() {
            try {
                // Load user info from file
                user_info = File.ReadAllLines("account.txt");
            } catch (Exception error) {
                // Handle file read error
                Console.WriteLine("------------------------\n{0}\n---------------------",error.Message);
                Environment.Exit(1);
            }
        }

        public void UserAccount() {
            Console.WriteLine("Login");
            Console.Write("User name:");
            string user_name = Console.ReadLine();
            Console.Write("Password:");
            string password = Console.ReadLine();
            Console.WriteLine("\n\n");

            if (user_name == user_info[0] && password == user_info[3]) {
                Menu();
            } else {
                Console.WriteLine("Could not login\nWrong credentials!\n------------------");
            }
        }

        private int AccountFunds() {
            int funds = Convert.ToInt32(user_info[5]);
            Console.Write("Account Funds\n$:{0}\n\n", funds);
            Menu(); // Return to main menu
            return funds;
        }

        private void Deposit() {
            Console.WriteLine("------------------\nCurrent funds in account:\n$ {0}\n---------------------\n", user_info[5]);
            Console.Write("Deposit amount:$ ");
            string funds_str = Console.ReadLine();
            if (int.TryParse(funds_str, out int funds) && funds > 0) {
                try {
                    if (File.Exists("account.txt")) {
                        int new_funds = Convert.ToInt32(user_info[5]) + funds;
                        user_info[5] = Convert.ToString(new_funds);
                        File.WriteAllLines("account.txt", user_info);
                    }
                } catch (Exception error) {
                    // Handle file write error
                    Console.WriteLine("------------------------\n{0}\n---------------------", error.Message);
                    Menu();
                }
                Console.WriteLine("Your funds have been deposited successfully!");
                Console.WriteLine("New balance:\n$ {0}\n", user_info[5]);
            } else {
                Console.WriteLine("-----------------------\nEnter amount > 0");
                Deposit();
            }
            Menu();
        }

        private void Withdraw() {
            Console.WriteLine("----------------------------------");
            Console.WriteLine("Current funds:$ {0}\nLimit:$ {1}", user_info[5], limit);
            Console.WriteLine("----------------------------------");
            int current_funds = Convert.ToInt32(user_info[5]);

            Console.Write("Withdraw:$ ");
            string withdraw_str = Console.ReadLine();
            if (int.TryParse(withdraw_str, out int withdraw) && withdraw > 0 && withdraw < limit && current_funds - withdraw >= 0) {
                int new_funds = current_funds - withdraw;
                user_info[5] = Convert.ToString(new_funds);
                try {
                    File.WriteAllLines("account.txt", user_info);
                } catch (Exception error) {
                    // Handle file write error
                    Console.WriteLine("------------------------\n{0}\n---------------------", error.Message);
                    Menu();
                }
                Console.WriteLine("Funds successfully withdrawn!\nNew Account balance:$ {0}\n\n", user_info[5]);
            } else {
                Console.Write("--------------------------------------\n");
                Console.WriteLine("Your request is invalid or above the limit\nRequest:$ {0}\nCurrent balance:$ {1}\nLimit:$ {2}",withdraw,user_info[5],limit);
                    Console.Write("--------------------------------------\n\n");
                    withdraw = 0; //reseting request
                    Withdraw();
            }
            Console.WriteLine("Funds successfully withdrawn!\nNew Account ballance:$ {0}\n\n",user_info[5]);
            Menu();
        }

        private void ChangePassword(){
            Console.Write("Change your password here\nKeep your password safe\n\n");
            if (user_info.Length < 4){
                Console.WriteLine("User info array has less than 4 elements.");
                return;
            }
            string current_password = user_info[3];
            Console.Write("Confirm old password: ");
            string confirm = Console.ReadLine();
            if(confirm == current_password){
                Console.Write("\nEnter new password: ");
                string password = Console.ReadLine();

            if (string.IsNullOrEmpty(password)){
                Console.WriteLine("New password cannot be empty.");
                return;
            }

            if(password.Length <= current_password.Length){
                Console.WriteLine("New password must be longer than old password.");
                return;
            }

            if(password == current_password){
                Console.Write("------------------------------------------------\n");
                Console.Write("New password cannot be the same as old password\n");
                Console.Write("--------------------------------------------------\n");
                return;
            }
            user_info[3] = password;
            } else {
                Console.Write("\n----------------\nPassword mismatched\n--------------------\n");
                return;
            }    
            try {
                File.WriteAllLines("account.txt",user_info);
            } catch(Exception error) {
                Console.WriteLine("Error while writing to file: {0}", error.Message);
            }
            
            Console.WriteLine("Your password has been changed successfully!\n");
            Menu();
        }

        public void Menu() {
            Console.WriteLine("Choose an option\n1: Check account funds\n2: Deposit funds\n3: Exit");
            string option = Console.ReadLine();
            switch (option) {
                case "1":
                    AccountFunds();
                    break;
                case "2":
                    Deposit();
                    break;
                case "3":
                    Console.WriteLine("Goodbye");
                    break;
                default:
                    Console.WriteLine("-----------------\nEnter valid option\n------------------");
                    break;
            }
        }
    }

    class Run {
        static void Main(String [] args){
            Console.Title = "Bank";
            CreateAccount signup = new CreateAccount();
            Account user = new Account();
            int option = 0;
            while (option != 1 && option != 2){
                Console.Write("\n\n1-> login\n2-> Register\n>>");
                string input = Console.ReadLine();
                
                if (!int.TryParse(input, out option)){
                    Console.WriteLine("Invalid input. Please enter a valid option.");
                }
            }
            
            if (option == 1){
                user.UserAccount();
            } else {
                signup.SetAccount();
            }
            Console.Write("Press any key to exit...");
            Console.ReadKey();
        }
    }
}