using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection receivedConn = ConnectionToCarRentalDb();

            bool exit = false;

            while (!exit)
            {
                Console.Clear();

                Console.WriteLine("\t\t Welcome in car rental\n");

                Console.WriteLine("Choose your option:");
                Console.WriteLine("1. Sign in");
                Console.WriteLine("2. Creat account");
                Console.WriteLine("3. Exit");
                string ChoosenOption = Console.ReadLine();

                switch (ChoosenOption)
                {
                    case "1": SignInSheet(receivedConn); break;
                    case "2": CreatAccountSheet(receivedConn); break;
                    case "3": exit = true; break;
                    default: Console.WriteLine("Incorrect option! Press any key to continue"); Console.ReadKey(); break;
                }

            }
            receivedConn.Close();
        }

        private static SqlConnection ConnectionToCarRentalDb()
        {
            string connection_params = @"Server=DESKTOP-O4NHD5U\SQLEXPRESS;" +  //Here change server name 
                "Database=CarRental;Trusted_Connection=True;";

            SqlConnection connection = new SqlConnection(connection_params);

            try
            {
                connection.Open();
            }
            catch (Exception Ex)
            {

                Console.WriteLine(Ex.Message);
            }

            return connection;
        }

        private static void CreatAccountSheet(SqlConnection connection)
        {
            Console.Clear();

            bool CorrectUserData = false;

            while (!CorrectUserData)
            {
                Console.WriteLine("Login:");
                string inputLogin = Console.ReadLine();

                Console.WriteLine("Password:");
                string inputPassword = Console.ReadLine();

                Console.WriteLine("Confirm Password:");
                string inputConfirmPassword = Console.ReadLine();

                Console.WriteLine("Name:");
                string inputName = Console.ReadLine();

                Console.WriteLine("Surname:");
                string inputSurname = Console.ReadLine();

                Console.WriteLine("User Name:");
                string inputUserName = Console.ReadLine();


                SqlCommand commandUserName = new SqlCommand($"SELECT UserName FROM Clients WHERE UserName = '{inputUserName}'", connection);
                SqlCommand commandLogin = new SqlCommand($"SELECT Login FROM Clients WHERE Login = '{inputLogin}'", connection);


                if (commandUserName.ExecuteScalar() != null)
                {
                    Console.WriteLine("The user name already existing. Try another");
                    Console.ReadKey();
                    Console.Clear();
                    continue;
                }
                else if (commandLogin.ExecuteScalar() != null)
                {
                    Console.WriteLine("The login used already existing. Try another");
                    Console.ReadKey();
                    Console.Clear();
                    continue;
                }
                else
                {
                    if (inputPassword != inputConfirmPassword)
                    {
                        Console.WriteLine("The passwords do not match. Try again");
                        Console.ReadKey();
                        Console.Clear();
                        continue;
                    }
                    else
                    {
                        SqlCommand PutUserToDatabas = new SqlCommand($"INSERT INTO Clients (Name,Surname,Login, Password, UserName) " +
                                                                     $"VALUES ('{inputName}','{inputSurname}','{inputLogin}','{inputPassword}','{inputUserName}')", connection);
                        PutUserToDatabas.ExecuteNonQuery();

                        Console.WriteLine("\nSuccessfule account creation. Press any key to go back to first page");
                        CorrectUserData = true;
                        Console.ReadKey();
                        Console.Clear();
                    }
                }

            }
        }

        private static void SignInSheet(SqlConnection connection)
        {
            List<string> UserItemlList = new List<string>();
            bool CorrectLoginPassword = false;

            while (!CorrectLoginPassword)
            {
                Console.Clear();

                Console.WriteLine("Login:");
                string inputLogin = Console.ReadLine();

                Console.WriteLine("Password:");
                string inputPassword = Console.ReadLine();

                SqlCommand command = new SqlCommand($"SELECT * FROM Clients WHERE Login = '{inputLogin}'", connection);

                if (command.ExecuteScalar() != null)
                {
                    string readPassword = "";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            readPassword = (string)reader[4];
                        }

                    }

                    if (readPassword == inputPassword)
                    {
                        Console.WriteLine("Successful sing in");
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserItemlList.Add(String.Format("{0}", reader[0])); //IDClient
                                UserItemlList.Add((string)reader[1]); //Client Name
                                UserItemlList.Add((string)reader[5]); //Client Username
                            }
                        }
                        
                        CorrectSignIn(UserItemlList, connection);

                        CorrectLoginPassword = true;
                        Console.ReadKey();
                        Console.Clear();
                    }
                    else
                    {
                        Console.WriteLine("Wrong password");
                        Console.ReadKey();
                        Console.Clear();
                        continue;
                    }
                    
                }
                else
                {
                    Console.WriteLine("Wrong login");
                    Console.ReadKey();
                    Console.Clear();
                    continue;
                }
            }
        }

        private static void CorrectSignIn(List<string> userItemlList, SqlConnection connection)
        {
            Console.Clear();
            bool Logout = false;

            while (!Logout)
            {
                Console.Clear();
                Console.WriteLine("\t\t Welcome {0} in Car rental\n", userItemlList[1]);

                Console.WriteLine("Choose your option:");
                Console.WriteLine("1. View all cars");
                Console.WriteLine("2. View available cars");
                Console.WriteLine("3. Rent a car");
                Console.WriteLine("4. Give back car");
                Console.WriteLine("5. Logout");

                string ChoosenOption = Console.ReadLine();

                switch (ChoosenOption)
                {
                    case "1": ViewAllCars(connection); break;
                    case "2": ViewAvailableCars(connection); break;
                    case "3": RentACar(userItemlList[0], connection); break;
                    case "4": GiveBackCar(userItemlList[0], connection); break;
                    case "5": Logout = true; break;
                    default: Console.WriteLine("Incorrect option! Press any key to continue"); Console.ReadKey(); break;
                }

            }

        }

        private static void GiveBackCar(string IDClient, SqlConnection connection)
        {
            Console.Clear();
            SqlCommand commandNumberCarsRented = new SqlCommand($"SELECT COUNT(*) IDCar FROM CarsRentedInfo WHERE " +
                                                                $"IDClient = {int.Parse(IDClient)} AND IsGivenBack = 'No'",connection);
            var numberCarsRented = Convert.ToInt32(commandNumberCarsRented.ExecuteScalar());

            SqlCommand commandWhatCarsRented = new SqlCommand($"SELECT IDRentedCar, IDCar FROM CarsRentedInfo WHERE IDClient = {int.Parse(IDClient)} " +
                                                              $"AND IsGivenBack = 'No'", connection);
            bool UserChoose = false;
            
            if (numberCarsRented > 0)
            {
                var IDRentedCar = new List<int>();
                var IDCarRented = new List<int>();

                using (SqlDataReader reader = commandWhatCarsRented.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //IDRentedCar.Add(int.Parse(String.Format("{0}",reader[0])));
                        IDRentedCar.Add(reader.GetInt32(0));
                        IDCarRented.Add(reader.GetInt32(1));
                    }
                }

                Console.WriteLine("Car ID | Make | Model | Engine | Year \n");

                for (int i = 0; i < numberCarsRented; i++)
                {
                    SqlCommand commandRentedCarInfo = new SqlCommand($"SELECT * FROM Cars WHERE IDCar = {IDCarRented[i]}", connection);
                    commandRentedCarInfo.ExecuteNonQuery();

                    using (SqlDataReader reader = commandRentedCarInfo.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(String.Format("{0} | {1} | {2} | {3} | {4}", reader[0], reader[1], 
                                                                                           reader[2], reader[3], reader[4]));
                        }
                    }
                }
                

                while (!UserChoose)
                {
                    Console.WriteLine("\nChoose ID car to return or press 'x' to go back");
                    var ChoosenCar = Console.ReadLine();

                    if (ChoosenCar == "x")
                    {
                        UserChoose = true;
                    }
                    else
                    {
                        int number;
                        bool ChoiceIsInt = int.TryParse(ChoosenCar, out number);
                        
                        if (!ChoiceIsInt)
                        {
                            Console.WriteLine("\nSomething goes wrong. Try again. Press any key to continue");
                            Console.ReadKey();
                        }
                        else
                        {
                            int IDRC = 0;
                            bool CorrectChoice = false;

                            for (int i = 0; i < numberCarsRented; i++)
                            {
                                if (ChoosenCar == Convert.ToString(IDCarRented[i]))
                                {
                                    CorrectChoice = true;
                                    IDRC = i;
                                    i = numberCarsRented;
                                }
                            }

                            if (CorrectChoice == false)
                            {
                                Console.WriteLine("\nIncorrect  car ID. Press any key to continue");
                                Console.ReadKey();
                            }
                            else
                            {
                                var commandGiveBackCar = new SqlCommand($"UPDATE CarsRentedInfo SET IsGivenBack = 'Yes' WHERE " +
                                                                               $"IDRentedCar = {IDRentedCar[IDRC]}", connection);
                                commandGiveBackCar.ExecuteNonQuery();

                                var commandUpdateCars = new SqlCommand($"UPDATE Cars SET CarAvailable = 'Yes' WHERE " +
                                                                       $"IDCar = {IDCarRented[IDRC]}", connection);
                                commandUpdateCars.ExecuteNonQuery();

                                UserChoose = true;

                                Console.WriteLine("\nSuccessful return of the car");
                                Console.ReadKey();
                            }
                        }
                    }
                }

            }
            else
            {
                Console.WriteLine("You don't have a car to return\n");
                Console.WriteLine("Press any key to go back to previous page");
                Console.ReadKey();
            }
        }

        private static void RentACar(string IDClient, SqlConnection connection)
        {
            SqlCommand command = new SqlCommand("SELECT * FROM Cars WHERE CarAvailable = 'yes'", connection);

            bool UserChoose = false;
            while (!UserChoose)
            {
                Console.Clear();
                Console.WriteLine("Car ID | Make | Model | Engine | Year \n");

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(String.Format("{0} | {1} | {2} | {3} | {4}", reader[0], reader[1], reader[2], reader[3], reader[4]));
                    }
                }

                Console.WriteLine("\nChoose ID car to rent or press 'x' to go back");
                var ChoosenCar = Console.ReadLine();

                if (ChoosenCar == "x")
                {
                    UserChoose = true;
                }
                else 
                {
                    bool CheckIfCarRent = false;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (!CheckIfCarRent)
                        {
                            while (reader.Read())
                            {
                                if (String.Format("{0}", reader[0]) == ChoosenCar)
                                {
                                    Console.WriteLine("\nCar successfuly rented");
                                    CheckIfCarRent = true;
                                    UserChoose = true;
                                    break;
                                }
                            }

                            if (CheckIfCarRent == false)
                            {
                                Console.WriteLine("\nSomething goes wrong. Try again. Press any key to continue");
                                Console.ReadKey();
                                break;
                            }
                        }
                    }
                    if (CheckIfCarRent == true)
                    {
                        var commandRentCar = new SqlCommand($"UPDATE Cars SET CarAvailable = 'No' WHERE IDCar ={ChoosenCar}", connection);
                        commandRentCar.ExecuteNonQuery();

                        var commandAddRentCarInfo = new SqlCommand($"INSERT INTO CarsRentedInfo (IDClient,IDCar,IsGivenBack)" +
                                                                   $"VALUES ({int.Parse(IDClient)},{int.Parse(ChoosenCar)},'No')", 
                                                                   connection);
                        commandAddRentCarInfo.ExecuteNonQuery();
                    }
                }
                
            }
            Console.ReadKey();
        }

        private static void ViewAvailableCars(SqlConnection connection)
        {
            Console.Clear();
            
            SqlCommand command = new SqlCommand("SELECT * FROM Cars WHERE CarAvailable = 'yes'", connection);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                Console.WriteLine("Make | Model | Engine | Year \n");

                while (reader.Read())
                {
                    Console.WriteLine(String.Format("{0} | {1} | {2} | {3}", reader[1], reader[2], reader[3], reader[4]));
                }
            }

            Console.WriteLine("\nPress any key to go back to previous page");
            Console.ReadKey();
        }

        private static void ViewAllCars(SqlConnection connection)
        {
            Console.Clear();

            SqlCommand command = new SqlCommand("SELECT * FROM Cars", connection);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                Console.WriteLine("Make | Model | Engine | Year \n");

                while (reader.Read())
                {
                    Console.WriteLine(String.Format("{0} | {1} | {2} | {3}", reader[1], reader[2], reader[3], reader[4]));
                }
            }

            Console.WriteLine("\nPress any key to go back to previous page");
            Console.ReadKey();
        }
    }
}

