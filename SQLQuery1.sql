--CREATE TABLE Users (
--    user_id     INT IDENTITY PRIMARY KEY,
--    user_name   NVARCHAR(100) NOT NULL,
--    email       NVARCHAR(150) NOT NULL
--);


--CREATE TABLE UserProfile (
--    profile_id  INT IDENTITY PRIMARY KEY,
--    user_id     INT NOT NULL UNIQUE,
--    bio         NVARCHAR(255),
--    birth_date  DATE,
--    CONSTRAINT FK_UserProfile_User
--        FOREIGN KEY (user_id) REFERENCES Users(user_id)
--);


--CREATE TABLE Roles (
--    role_id INT IDENTITY PRIMARY KEY,
--    role_name NVARCHAR(50) NOT NULL
--);


--CREATE TABLE UserRole (
--    user_id INT NOT NULL,
--    role_id INT NOT NULL,
--    CONSTRAINT PK_UserRole PRIMARY KEY (user_id, role_id),
--    CONSTRAINT FK_UserRole_User FOREIGN KEY (user_id) REFERENCES Users(user_id),
--    CONSTRAINT FK_UserRole_Role FOREIGN KEY (role_id) REFERENCES Roles(role_id)
--);


--CREATE TABLE Orders (
--    order_id INT IDENTITY PRIMARY KEY,
--    user_id  INT NOT NULL,
--    total    DECIMAL(10,2),
--    order_date DATETIME DEFAULT GETDATE(),
--    CONSTRAINT FK_Orders_User FOREIGN KEY (user_id) REFERENCES Users(user_id)
--);



--INSERT INTO Users (user_name, email)
--VALUES ('Erick', 'erick@test.com'),
--       ('Ana', 'ana@test.com');

--INSERT INTO UserProfile (user_id, bio, birth_date)
--VALUES (1, 'Dev Senior', '1990-05-10'),
--       (2, 'QA Engineer', '1995-08-20');

--INSERT INTO Roles (role_name)
--VALUES ('Admin'), ('User'), ('Manager');

--INSERT INTO UserRole (user_id, role_id)
--VALUES (1, 1), (1, 2), (2, 2);

--INSERT INTO Orders (user_id, total)
--VALUES (1, 250.00),
--       (1, 500.00),
--       (2, 120.00);



--CREATE PROCEDURE sp_User_With_Profile
--    @UserId INT
--AS
--BEGIN
--    SELECT user_id, user_name, email
--    FROM Users
--    WHERE user_id = @UserId;

--    SELECT profile_id, user_id, bio, birth_date
--    FROM UserProfile
--    WHERE user_id = @UserId;
--END;



--CREATE PROCEDURE sp_User_With_Orders
--    @UserId INT
--AS
--BEGIN
--    SELECT user_id, user_name, email
--    FROM Users
--    WHERE user_id = @UserId;

--    SELECT order_id, user_id, total, order_date
--    FROM Orders
--    WHERE user_id = @UserId;
--END;



--CREATE PROCEDURE sp_Users_With_Roles
--AS
--BEGIN
--    SELECT user_id, user_name, email
--    FROM Users;

--    SELECT role_id, role_name
--    FROM Roles;

--    SELECT user_id, role_id
--    FROM UserRole;
--END;


--CREATE PROCEDURE sp_Orders_With_User
--AS
--BEGIN
--    SELECT order_id, user_id, total, order_date
--    FROM Orders;

--    SELECT user_id, user_name, email
--    FROM Users;
--END;
