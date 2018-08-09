﻿using Apps72.Dev.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace Data.Core.Tests
{
    [TestClass]
    public class FluentQueryTests
    {
        #region INITIALIZATION

        private SqlConnection _connection;

        [TestInitialize]
        public void Initialization()
        {
            _connection = new SqlConnection(Configuration.CONNECTION_STRING);
            _connection.Open();
        }

        #endregion

        [TestMethod]
        public void ForSql_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                int count = cmd.Query()
                               .ForSql("SELECT COUNT(*) FROM EMP")
                               .ExecuteScalar<int>();

                Assert.AreEqual(14, count);
            }
        }

        [TestMethod]
        public void CompactFluent_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                int count = cmd.Query("SELECT COUNT(*) FROM EMP")
                               .ExecuteScalar<int>();

                Assert.AreEqual(14, count);
            }
        }

        [TestMethod]
        public void CompactFluent_WithParameters_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                int count = cmd.Query("SELECT COUNT(*) FROM EMP WHERE EMPNO > @ID", 
                                      new { ID = 10 })
                               .ExecuteScalar<int>();

                Assert.AreEqual(14, count);
            }
        }

        [TestMethod]
        public void Parameters_NameValue_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                int count = cmd.Query("SELECT COUNT(*) FROM EMP WHERE EMPNO > @ID")
                               .AddParameter("ID", 10)
                               .ExecuteScalar<int>();

                Assert.AreEqual(14, count);
            }
        }

        [TestMethod]
        public void Parameters_NameValueDbtype_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                int count = cmd.Query("SELECT COUNT(*) FROM EMP WHERE EMPNO > @ID")
                               .AddParameter("ID", 10, System.Data.DbType.Int32)
                               .ExecuteScalar<int>();

                Assert.AreEqual(14, count);
            }
        }

        [TestMethod]
        public void Parameters_Dynamic_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                int count = cmd.Query("SELECT COUNT(*) FROM EMP WHERE EMPNO > @ID")
                               .AddParameter(new { ID = 10 })
                               .ExecuteScalar<int>();

                Assert.AreEqual(14, count);
            }
        }

        [TestMethod]
        public void ExecuteScalar_Simple_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                int count = cmd.Query("SELECT COUNT(*) FROM EMP")
                               .ExecuteScalar<int>();

                Assert.AreEqual(14, count);
            }
        }

        [TestMethod]
        public void ExecuteScalar_Object_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                object count = cmd.Query("SELECT COUNT(*) FROM EMP")
                                  .ExecuteScalar();

                Assert.AreEqual(14, count);
            }
        }

        [TestMethod]
        public void ExecuteRow_Simple_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                var emp = cmd.Query("SELECT * FROM EMP WHERE EMPNO = 7369")
                             .ExecuteRow<EMP>();

                Assert.AreEqual("SMITH", emp.EName);
            }
        }

        [TestMethod]
        public void ExecuteRow_Lambda_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                var emp = cmd.Query("SELECT EName FROM EMP WHERE EMPNO = 7369")
                             .ExecuteRow(new { EName = String.Empty});

                Assert.AreEqual("SMITH", emp.EName);
            }
        }

        [TestMethod]
        public void ExecuteTable_Simple_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                var employees = cmd.Query("SELECT * FROM EMP ")
                                   .ExecuteTable<EMP>();
                var emp = employees.First(i => i.EmpNo == 7369);

                Assert.AreEqual("SMITH", emp.EName);
            }
        }

        [TestMethod]
        public void ExecuteTable_Lambda_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                var employees = cmd.Query("SELECT EMPNO, ENAME FROM EMP")
                                   .ExecuteTable(new { EmpNo = 0, EName = String.Empty });
                var emp = employees.First(i => i.EmpNo == 7369);

                Assert.AreEqual("SMITH", emp.EName);
            }
        }


        #region GET DBCOMMAND


        private IDatabaseCommand GetDatabaseCommand(DbConnection connection)
        {
            return new DatabaseCommand(connection);
        }

        private IDatabaseCommand GetDatabaseCommand(DbConnection connection, DbTransaction transaction)
        {
            return new DatabaseCommand(connection, transaction);
        }

        private IDatabaseCommand GetDatabaseCommand(DbConnection connection, string commandText)
        {
            return new DatabaseCommand(connection, commandText);
        }

        #endregion
    }
}