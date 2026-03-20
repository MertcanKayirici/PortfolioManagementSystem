# 🚀 Portfolio Management System

A modern and fully dynamic **portfolio management system** built with **ASP.NET MVC, Entity Framework, and SQL Server**.

This project includes a **powerful admin panel** that allows full control over portfolio content such as projects, skills, messages, and site settings.

---

## 🎬 Demo

> Admin panel and portfolio system in action

![Portfolio Demo](Screenshots/Portfolio.gif)

---

## ✨ Key Features

- 🔐 Authentication-based Admin Panel  
- 📂 Full CRUD Operations (Projects, Skills, Messages, etc.)  
- 🧩 Dynamic Project Detail Sections  
- ⚡ Real-time Preview System (Admin Panel)  
- 📊 Relational Database Design (SQL Server)  
- 🎨 Responsive UI (Bootstrap 5)  
- 💬 Contact & Message Management  
- 📈 Login Activity Logging System  

---

## 🛠️ Tech Stack

- ASP.NET MVC (.NET Framework)  
- Entity Framework  
- Microsoft SQL Server  
- Bootstrap 5  
- JavaScript (AJAX, DOM Manipulation)  
- HTML5 / CSS3  

---

## 🎥 Feature Demonstrations

### 🔐 Authentication System
![Login Demo](Screenshots/Portfolio_Admin_Login.gif)

### ⚙️ Admin Dashboard & Management
![Admin Panel](Screenshots/Portfolio_Admin_Genel.gif)

### ✏️ Content Management & Live Preview
![Content Demo](Screenshots/Portfolio_Admin_Icerik.gif)

---

## 📸 Screenshots

### 🌐 Public Interface

#### 🏠 Homepage
![Homepage](Screenshots/0_homepage.png)

#### 🔍 Project Detail (Empty State)
![Project Detail Empty](Screenshots/20_project_detail_empty.png)

#### 🔍 Project Detail (Full View)
![Project Detail Full](Screenshots/21_project_detail_full.png)

---

### ⚙️ Admin Panel

#### 📊 Dashboard
![Dashboard](Screenshots/1_dashboard.png)

#### 🖼️ Site Settings
![Site Settings](Screenshots/2_site_settings.png)

#### 👤 About Section
![About](Screenshots/3_about_edit.png)

#### ⚡ Real-time Preview System
![Live Preview](Screenshots/4_contact_live_preview.png)

#### 🧠 Skills Management
![Skills](Screenshots/5_skill_list.png)

#### ➕ Add Skill
![Skill Add](Screenshots/6_skill_add.png)

#### ✏️ Edit Skill
![Skill Edit](Screenshots/7_skill_edit.png)

#### 🎨 Skill Categories
![Skill Categories](Screenshots/8_skill_category_list.png)

#### 📂 Projects Management
![Projects](Screenshots/11_project_list.png)

#### ➕ Add Project
![Project Add](Screenshots/12_project_add.png)

#### ✏️ Edit Project
![Project Edit](Screenshots/13_project_edit.png)

#### 🧩 Project Sections
![Project Sections](Screenshots/14_project_section_list.png)

#### 💬 Messages
![Messages](Screenshots/17_messages.png)

#### 🔐 Login Logs
![Login Logs](Screenshots/18_login_logs.png)

#### 🔑 Login Page
![Login](Screenshots/19_login.png)

---

## 🧠 Database Design

![Database Diagram](Screenshots/database_diagram.png)

---

## ⚡ Highlight Feature

One of the standout features of this project is the **real-time preview system** in the admin panel.

While editing content such as contact information or project details, users can instantly preview changes before saving them.  
This improves user experience and reduces potential errors.

---

## ⚙️ Installation

### 1. Clone the repository
```bash
git clone https://github.com/MertcanKayirici/PortfolioManagementSystem.git
```

### 2. Open the project
Open the solution file (`.sln`) with Visual Studio.

### 3. Configure database connection
Update your connection string in **Web.config**:

⚠️ Replace `YOUR_SERVER_NAME` with your local SQL Server instance.

```xml
<connectionStrings>
  <add name="PortfolioDbEntities"
       connectionString="Data Source=YOUR_SERVER_NAME;Initial Catalog=PortfolioDb;Integrated Security=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

### 4. Create the database
Run the SQL script located in the `Database` folder.

### 5. Run the project
Press `F5` or click **Start** in Visual Studio.

---

## 📌 Important Notes

- Ensure SQL Server is running  
- Update connection string before running  
- Do not share sensitive credentials  

---

## 📂 Project Structure

- Controllers → MVC Controllers  
- Models → Entity Framework Models  
- Views → Razor Views  
- Content → CSS, images, static files  
- Scripts → JavaScript files  

---

## 👨‍💻 Developer

**Mertcan Kayırıcı**

- Backend-focused Full Stack Developer  
- ASP.NET MVC & SQL Server  

---

## ⭐ Project Purpose

This project was developed to simulate a **real-world portfolio management system**, focusing on clean architecture, dynamic content handling, and a modern admin experience.