# 🚀 Portfolio Management System

<p align="center">
  <img src="https://img.shields.io/badge/.NET-Framework-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/ASP.NET-MVC-5C2D91?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/SQL-Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" />
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Entity-Framework-512BD4?style=for-the-badge" />
  <img src="https://img.shields.io/badge/Bootstrap-5-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white" />
  <img src="https://img.shields.io/badge/JavaScript-AJAX-F7DF1E?style=for-the-badge&logo=javascript&logoColor=black" />
</p>

---

> ⚡ A dynamic portfolio management system with secure authentication, real-time content updates, and a fully customizable admin panel

This project provides a complete solution for managing portfolio content through a powerful **admin panel**, while delivering a clean and responsive **public-facing website**.

---

### 🎬 Demo GIFs

| 🔐 Login Flow |
|------------|
| ![Admin Login Flow](Screenshots/Portfolio_Admin_Login.gif) |

| 🖥️ General Pages | 📦 Content Pages |
|------------|-------------|
| ![Admin General Pages](Screenshots/Portfolio_Admin_Genel.gif) | ![Content Management](Screenshots/Portfolio_Admin_Icerik.gif) |

---

## ✨ Features

### 🔐 Authentication & Security
- Secure login system  
- Password hashing (PBKDF2 - salted)  
- Login activity logging (IP + timestamp)  

### 📊 Admin Panel
- Full CRUD operations (Projects, Skills, Messages, etc.)  
- Dynamic dashboard overview  
- Real-time preview system  
- Site settings management  

### 🌐 Portfolio Website
- Dynamic project rendering  
- Responsive UI (Bootstrap 5)  
- Contact & message system  
- Clean and modern design  

---

## 🛠️ Tech Stack

- ASP.NET MVC (.NET Framework)  
- Entity Framework  
- Microsoft SQL Server  
- Bootstrap 5  
- JavaScript (AJAX, DOM Manipulation)  
- HTML5 / CSS3  

---

## 📸 Screenshots

### 🌐 Homepage

| Homepage |
|----------|
| ![Portfolio Homepage](Screenshots/0_homepage.png) |

---

### 📊 Dashboard & Settings

| Dashboard | Site Settings |
|----------|--------------|
| ![Admin Dashboard](Screenshots/1_dashboard.png) | ![Site Settings Panel](Screenshots/2_site_settings.png) |

---

### 👤 About & Live Preview

| About Edit | Live Preview |
|-----------|-------------|
| ![About Edit Page](Screenshots/3_about_edit.png) | ![Live Preview System](Screenshots/4_contact_live_preview.png) |

---

### 🛠️ Skills Management

| Skill List | Add Skill | Edit Skill |
|-----------|-----------|------------|
| ![Skill List](Screenshots/5_skill_list.png) | ![Add Skill](Screenshots/6_skill_add.png) | ![Edit Skill](Screenshots/7_skill_edit.png) |

---

### 🗂️ Skill Categories

| List | Add | Edit |
|------|-----|------|
| ![Category List](Screenshots/8_skill_category_list.png) | ![Add Category](Screenshots/9_skill_category_add.png) | ![Edit Category](Screenshots/10_skill_category_edit.png) |

---

### 📁 Projects Management

| Project List | Add Project | Edit Project |
|--------------|------------|--------------|
| ![Project List](Screenshots/11_project_list.png) | ![Add Project](Screenshots/12_project_add.png) | ![Edit Project](Screenshots/13_project_edit.png) |

---

### 🧩 Project Sections

| Section List | Add Section | Edit Section |
|--------------|------------|--------------|
| ![Section List](Screenshots/14_project_section_list.png) | ![Add Section](Screenshots/15_project_section_add.png) | ![Edit Section](Screenshots/16_project_section_edit.png) |

---

### 💬 Messages & Logs

| Messages | Login Logs |
|----------|------------|
| ![User Messages](Screenshots/17_messages.png) | ![Login Logs](Screenshots/18_login_logs.png) |

---

### 🔐 Authentication

| Login |
|------|
| ![Login Page](Screenshots/19_login.png) |

---

### 🌐 Project Detail Page

| Empty | Full |
|------|------|
| ![Empty Project Detail](Screenshots/20_project_detail_empty.png) | ![Full Project Detail](Screenshots/21_project_detail_full.png) |

---

### 🧠 Database Diagram

| ER Diagram |
|------------|
| ![Database Schema](Screenshots/database_diagram.png) |

---

## 🚀 Key Highlights

- Secure authentication with activity logging  
- Fully dynamic content management system  
- Real-time preview functionality  
- Clean and scalable database design  
- Admin panel with full CRUD operations
- Role-based admin panel structure

---

## 🏗️ Architecture

This project follows a **layered MVC architecture**:

- Controllers → Handle HTTP requests and application flow  
- Models → Represent database entities (Entity Framework)  
- Views → Razor-based UI rendering  
- Database → SQL Server relational structure  

The system is designed with **separation of concerns** and maintainability in mind.

---

## 🔄 How It Works

### 🌐 User Side
1. User visits the portfolio website  
2. Data is fetched from SQL Server via Entity Framework  
3. Controllers process the request  
4. Views render dynamic content  

### 🔐 Admin Panel
1. Admin logs in securely  
2. Performs CRUD operations  
3. Changes are instantly reflected on the website  

---

## ⚙️ Installation

### 1. Clone the repository
```bash
git clone https://github.com/MertcanKayirici/PortfolioManagementSystem.git
```
### 2. Open the project

Open the .sln file using Visual Studio

### 3. Create database

Create a database named:
```bash
PortfolioDb
```
### 4. Run SQL script

Execute:
```bash
Database/PortfolioDb.sql
```
### 5. Configure connection string

Update your `Web.config`:

```xml
<connectionStrings>
  <add name="PortfolioDb"
       connectionString="Data Source=YOUR_SERVER_NAME;Initial Catalog=PortfolioDb;Integrated Security=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```
- ⚠️ Make sure to replace YOUR_SERVER_NAME with your SQL Server instance name.

## 6. Run the project

Run the project using **Visual Studio (F5)** 🚀

---

## 📌 Important Notes
- Ensure SQL Server is running
- Update the connection string before running
- Do not share sensitive credentials

---

## 📂 Project Structure
- Controllers   → MVC Controllers  
- Models        → Entity Framework Models  
- Views         → Razor Views  
- Database      → SQL Scripts  
- Screenshots   → Images & GIF files  

---

## 👨‍💻 Developer

Mertcan Kayırıcı

Backend-focused Full Stack Developer
ASP.NET MVC & SQL Server
---

## 💡 Why This Project Matters

This project demonstrates the ability to build a full-featured content management system, not just a static portfolio website.

It includes:

- Secure authentication and user activity tracking  
- Dynamic content management via admin panel  
- Real-time preview and updates  
- Scalable database design with relational structure  

This makes it closer to real-world CMS systems used in production.

---

## ⭐ Project Purpose

This project was built to simulate a real-world portfolio CMS system and demonstrate:

- Secure authentication and authorization logic  
- Dynamic content management  
- Admin panel architecture  
- Scalable relational database design  
