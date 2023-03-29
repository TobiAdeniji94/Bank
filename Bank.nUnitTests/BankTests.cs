using NUnit.Framework;
using System;
using System.IO;

namespace Bank.Tests
{
    [TestFixture]
    public class BankTests
    {
        [Test]
        public void CreateAccount_ValidInput_CreatesAccount()
        {
            // Arrange
            var input = new StringReader("John\nDoe\n01/01/1980\npassword123\nSavings\n500\n");
            Console.SetIn(input);

            // Act
            var createAccount = new CreateAccount();
            createAccount.SetAccount();

            // Assert
            Assert.That(File.Exists("account.txt"), Is.True);
            var lines = File.ReadAllLines("account.txt");
            Assert.That(lines.Length, Is.EqualTo(6));
            Assert.That(lines[0], Is.EqualTo("John"));
            Assert.That(lines[1], Is.EqualTo("Doe"));
            Assert.That(lines[2], Is.EqualTo("01/01/1980"));
            Assert.That(lines[3], Is.EqualTo("password123"));
            Assert.That(lines[4], Is.EqualTo("Savings"));
            Assert.That(lines[5], Is.EqualTo("500"));
            File.Delete("account.txt");
        }

        [Test]
        public void CreateAccount_InvalidInput_RetriesInput()
        {
            // Arrange
            var input = new StringReader("John\nDoe\n01/01/1980\npassword123\nSavings\n100\n500\n");
            Console.SetIn(input);

            // Act
            var createAccount = new CreateAccount();
            createAccount.SetAccount();

            // Assert
            Assert.That(File.Exists("account.txt"), Is.False);
            StringAssert.Contains("Sorry, enter a deposit >= 500", Console.Out.ToString());
        }

        [Test]
        public void CreateAccount_FileWriteError_DisplaysErrorMessage()
        {
            // Arrange
            var input = new StringReader("John\nDoe\n01/01/1980\npassword123\nSavings\n500\n");
            Console.SetIn(input);
            File.WriteAllText("account.txt", "dummy content");

            // Act
            var createAccount = new CreateAccount();
            createAccount.SetAccount();

            // Assert
            StringAssert.Contains("The process cannot access the file", Console.Out.ToString());
            File.WriteAllText("account.txt", "");
        }

        [Test]
        public void Account_Login_ValidCredentials_DisplaysMenu()
        {
            // Arrange
            var input = new StringReader("John\npassword123\n");
            Console.SetIn(input);
            File.WriteAllLines("account.txt", new[] { "John", "Doe", "01/01/1980", "password123", "Savings", "500" });

            // Act
            var account = new Account();
            account.UserAccount();

            // Assert
            StringAssert.Contains("Welcome to the Bank", Console.Out.ToString());
            StringAssert.Contains("1-> Account Enquiry", Console.Out.ToString());
            StringAssert.Contains("2-> Deposit funds", Console.Out.ToString());
            StringAssert.Contains("3-> Withdraw funds", Console.Out.ToString());
            StringAssert.Contains("4-> Change password", Console.Out.ToString());
            File.Delete("account.txt");
        }

        [Test]
        public void Account_Login_InvalidCredentials_DisplaysErrorMessage()
        {
            // Arrange
            var input = new StringReader("John\nwrongpassword\n");
            Console.SetIn(input);
            File.WriteAllLines("account.txt", new[] { "John", "Doe", "01/01/1980", "password123", "Savings", "500" });

            // Act
            var account = new Account();
            account.UserAccount();

            // Assert
            StringAssert.Contains("Could not login", Console.Out.ToString());
            File.Delete("account.txt");
        }
    }
}
