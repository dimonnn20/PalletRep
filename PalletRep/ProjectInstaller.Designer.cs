﻿namespace PalletRep
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.AAAAAAAAAAAAAAAAAAAAAAAA = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller1
            // 
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            // 
            // AAAAAAAAAAAAAAAAAAAAAAAA
            // 
            this.AAAAAAAAAAAAAAAAAAAAAAAA.Description = "AAAAAAAAAAAAAAAAA";
            this.AAAAAAAAAAAAAAAAAAAAAAAA.DisplayName = "AAAAAAAAAAAAAAAAAAA";
            this.AAAAAAAAAAAAAAAAAAAAAAAA.ServiceName = "Service1";
            this.AAAAAAAAAAAAAAAAAAAAAAAA.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.AAAAAAAAAAAAAAAAAAAAAAAA.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.AAAAAAAAAAAAAAAAAAAAAAAA_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller1,
            this.AAAAAAAAAAAAAAAAAAAAAAAA});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller AAAAAAAAAAAAAAAAAAAAAAAA;
    }
}