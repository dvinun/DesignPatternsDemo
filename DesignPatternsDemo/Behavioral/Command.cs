using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dvinun.DesignPatterns.Behavioral
{
    class Command
    {
        public static void PlayDemo()
        {
            Account marysAccount = new Account("Mary", "ACN190209");
            Transaction depositToMarysAccount = new Deposit(marysAccount, 50);
            Transaction withdrawFromMarysAccount = new Withdraw(marysAccount, 10);

            TransactionManager transactionManager = new TransactionManager();
            transactionManager.Add(depositToMarysAccount);
            transactionManager.Add(withdrawFromMarysAccount);
            transactionManager.ExecuteAllTransactions();

            Account natesAccount = new Account("Nate", "ACN827899");
            Transaction transferfromMaryToNate = new Transfer(marysAccount, natesAccount, 10);

            transactionManager.Add(transferfromMaryToNate);
            transactionManager.ExecuteAllTransactions();

            transferfromMaryToNate = new Transfer(marysAccount, natesAccount, 100);
            transactionManager.Add(transferfromMaryToNate);
            TransactionResult result = transactionManager.Execute();

            if (result.Status == TransactionStatus.InsufficientFunds)
            {
                depositToMarysAccount = new Deposit(marysAccount, 200);
                transactionManager.Add(depositToMarysAccount);
                transactionManager.Execute();
            }

            // Try again the transfer transaction 
            transactionManager.RetryTransaction(result.TransactionId);

            // undo all the transactions
            transactionManager.UndoAllTransactions();
        }

        enum TransactionStatus
        {
            Pending = 0,
            InProgress = 1,
            Success = 2,
            Cancelled = 3,
            InsufficientFunds = 4,
            Undone = 5,
            Invalid = 6,
        }

        class TransactionResult
        {
            public TransactionStatus Status;
            public string TransactionId;

            public TransactionResult(string transactionId, TransactionStatus status)
            {
                this.TransactionId = transactionId;
                Status = status;
            }
        }

        // Invoker
        class TransactionManager
        {
            List<Transaction> transactions = new List<Transaction>();

            public void Add(Transaction transaction)
            {
                transactions.Add(transaction);
            }

            // Execute last transaction
            public TransactionResult Execute()
            {
                Transaction transaction = transactions.Last();
                return transaction.Execute();
            }

            // Execute specific transaction
            public TransactionResult RetryTransaction(string transactionId)
            {
                Transaction transaction = transactions.Find(item => item.GetTransactionId() == transactionId);

                if (transaction.GetStatus() == TransactionStatus.Success ||
                    transaction.GetStatus() == TransactionStatus.Undone)
                {
                    Console.WriteLine("Cant retry the transaction because the status is {0}", transaction.GetStatus().ToString());
                    return null;
                }

                // Since this transaction is executed. Create a new transaction and execute it.
                Transaction newTransaction = transaction.Clone();
                transactions.Add(newTransaction);

                // Also flag the transaction as invalid
                transaction.SetStatus(TransactionStatus.Invalid);

                return newTransaction.Execute();
            }

            public void ExecuteAllTransactions()
            {
                foreach (Transaction transaction in transactions)
                {
                    if (transaction.GetStatus() != TransactionStatus.Success &&
                       transaction.GetStatus() != TransactionStatus.Cancelled &&
                       transaction.GetStatus() != TransactionStatus.InProgress &&
                       transaction.GetStatus() != TransactionStatus.Undone)
                        transaction.Execute();
                }
            }

            internal void UndoAllTransactions()
            {
                for (int i = transactions.Count - 1; i >= 0; i--)
                {
                    Transaction transaction = transactions[i];
                    transaction.Undo();
                }
            }
        }

        // Receiver
        class Account
        {
            public string HolderName { get; set; }
            public string Number { get; set; }
            public double Balance { get; set; }

            public Account(string holderName, string number)
            {
                this.HolderName = holderName;
                this.Number = number;
                Balance = 0;
            }
        }

        // Command Abstract Class
        abstract class Transaction
        {
            protected Account account;
            protected Account toAccount;
            protected double money;
            protected string transactionId;
            protected TransactionStatus status;

            public Transaction()
            {
                this.transactionId = Guid.NewGuid().ToString();
                this.status = TransactionStatus.Pending;
            }

            public abstract TransactionResult Execute();
            public abstract TransactionResult Undo();
            public abstract Transaction Clone();

            public TransactionStatus GetStatus() { return status; }
            public string GetTransactionId() { return transactionId; }

            internal void SetStatus(TransactionStatus status)
            {
                this.status = status;
            }
        }

        // Concrete Command Class
        class Deposit : Transaction
        {
            public Deposit(Account account, double money) : base()
            {
                this.account = account;
                this.money = money;
            }

            public override Transaction Clone()
            {
                return new Deposit(account, money);
            }

            public override TransactionResult Undo()
            {
                Console.WriteLine("==== Undo Deposit : Begin ====");

                if (status != TransactionStatus.Success)
                {
                    Console.WriteLine("Cant undo the transaction because the status is {0}", status.ToString());
                    Console.WriteLine("==== Undo Deposit : End ====");
                    Console.WriteLine("");
                    return null;
                }

                this.status = TransactionStatus.InProgress;
                account.Balance -= money;
                Console.WriteLine("Undo Deposit to {0}'s account: ${1}. New balance: ${2}", account.HolderName, money, account.Balance);
                Console.WriteLine("==== Undo Deposit : End ====");
                Console.WriteLine("");
                this.status = TransactionStatus.Undone;
                return new TransactionResult(transactionId, this.status);
            }

            public override TransactionResult Execute()
            {
                this.status = TransactionStatus.InProgress;
                account.Balance += money;
                Console.WriteLine("==== Deposit : Begin ====");
                Console.WriteLine("Deposit to {0}'s account: ${1}. New balance: ${2}", account.HolderName, money, account.Balance);
                Console.WriteLine("==== Deposit : End ====");
                Console.WriteLine("");
                this.status = TransactionStatus.Success;
                return new TransactionResult(transactionId, this.status);
            }
        }

        // Concrete Command Class
        class Withdraw : Transaction
        {
            public Withdraw(Account account, double money) : base()
            {
                this.account = account;
                this.money = money;
            }

            public override Transaction Clone()
            {
                return new Withdraw(account, money);
            }

            public override TransactionResult Undo()
            {
                Console.WriteLine("==== Undo Withdraw : Begin ====");

                if (status != TransactionStatus.Success)
                {
                    Console.WriteLine("Cant undo the transaction because the status is {0}", status.ToString());
                    Console.WriteLine("==== Undo Withdraw : End ====");
                    Console.WriteLine("");
                    return null;
                }

                this.status = TransactionStatus.InProgress;
                Console.WriteLine("Undo Withdraw from {0}'s account: ${1}.", account.HolderName, money);

                account.Balance += money;
                this.status = TransactionStatus.Undone;

                Console.WriteLine("New Balance: ${0}", account.Balance);
                Console.WriteLine("==== Undo Withdraw : End ====");
                Console.WriteLine("");

                return new TransactionResult(transactionId, this.status);
            }

            public override TransactionResult Execute()
            {
                this.status = TransactionStatus.InProgress;
                Console.WriteLine("==== Withdraw : Begin ====");
                Console.WriteLine("Withdraw from {0}'s account: ${1}.", account.HolderName, money);

                if (account.Balance >= money)
                {
                    account.Balance -= money;
                    this.status = TransactionStatus.Success;
                }
                else
                {
                    this.status = TransactionStatus.InsufficientFunds;
                    Console.WriteLine("Insufficient funds.");
                }

                Console.WriteLine("New Balance: ${0}", account.Balance);
                Console.WriteLine("==== Withdraw : End ====");
                Console.WriteLine("");

                return new TransactionResult(transactionId, this.status);
            }
            // Concrete Command Class
        }

        // Concrete Command Class
        class Transfer : Transaction
        {
            public Transfer(Account fromAccount, Account toAccount, double money) : base()
            {
                this.account = fromAccount;
                this.toAccount = toAccount;
                this.money = money;
            }

            public override Transaction Clone()
            {
                return new Transfer(account, toAccount, money);
            }

            public override TransactionResult Undo()
            {
                Console.WriteLine("==== Undo Transfer : Begin ====");

                if (status != TransactionStatus.Success)
                {
                    Console.WriteLine("Cant undo the transaction because the status is {0}", status.ToString());
                    Console.WriteLine("==== Undo Transfer : End ====");
                    Console.WriteLine("");
                    return null;
                }

                this.status = TransactionStatus.InProgress;
                Console.WriteLine("Undo Transfer from {0}'s to {1}'s account: ${2}.", account.HolderName, toAccount.HolderName, money);

                this.account.Balance += money;
                this.toAccount.Balance -= money;
                this.status = TransactionStatus.Undone;

                Console.WriteLine("Balance {0}'s Account: ${1}", account.HolderName, account.Balance);
                Console.WriteLine("Balance {0}'s Account: ${1}", toAccount.HolderName, toAccount.Balance);
                Console.WriteLine("==== Undo Transfer : End ====");
                Console.WriteLine("");

                return new TransactionResult(transactionId, this.status);
            }

            public override TransactionResult Execute()
            {
                this.status = TransactionStatus.InProgress;

                Console.WriteLine("==== Transfer : Begin ====");
                Console.WriteLine("Transfer from {0}'s to {1}'s account: ${2}.", account.HolderName, toAccount.HolderName, money);

                if (account.Balance >= money)
                {
                    this.account.Balance -= money;
                    this.toAccount.Balance += money;
                    this.status = TransactionStatus.Success;
                }
                else
                {
                    this.status = TransactionStatus.InsufficientFunds;
                    Console.WriteLine("Insufficient funds on source account.");
                }

                Console.WriteLine("Balance {0}'s Account: ${1}", account.HolderName, account.Balance);
                Console.WriteLine("Balance {0}'s Account: ${1}", toAccount.HolderName, toAccount.Balance);
                Console.WriteLine("==== Transfer : End ====");
                Console.WriteLine("");

                return new TransactionResult(transactionId, this.status);
            }
        }
    }
}
