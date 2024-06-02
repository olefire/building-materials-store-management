using System;
using System.Drawing;
using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.Views;

namespace BuildingMaterialsStoreManagement.Internal.Utils
{
    public static class ButtonUtils
    {
        
        private static void NavigateBack(FormNavigator formNavigator, Form currentForm)
        {
            formNavigator.OpenHomeForm();
            currentForm.Close();
        }
        
        public static Button CreateBackButton(FormNavigator fm, Form f)
        {
            var backButton = new Button
            {
                Text = "На главную страницу",
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                BackColor = Color.FromArgb(70, 130, 180), // SteelBlue
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Size = new Size(200, 30),
                Margin = new Padding(5)
            };
            backButton.Click += (sender, args) => NavigateBack(fm, f);

            backButton.MouseEnter += (s, e) => backButton.BackColor = Color.FromArgb(100, 149, 237); // CornflowerBlue
            backButton.MouseLeave += (s, e) => backButton.BackColor = Color.FromArgb(70, 130, 180); // SteelBlue

            return backButton;
        }
        
        public static Button CreateButton(string text, EventHandler clickHandler)
        {
            var button = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                BackColor = Color.FromArgb(70, 130, 180), // SteelBlue
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Size = new Size(300, 80),
                Padding = new Padding(5),
                Margin = new Padding(5)
            };
            button.Click += clickHandler;

            button.MouseEnter += (s, e) => button.BackColor = Color.FromArgb(100, 149, 237); // CornflowerBlue
            button.MouseLeave += (s, e) => button.BackColor = Color.FromArgb(70, 130, 180); // SteelBlue

            return button;
        }
        
        public static Button CreateSmallButton(string text, EventHandler clickHandler)
        {
            var button = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Size = new Size(200, 60),
            };
            button.Click += clickHandler;

            button.MouseEnter += (s, e) => button.BackColor = Color.FromArgb(100, 149, 237); // CornflowerBlue
            button.MouseLeave += (s, e) => button.BackColor = Color.FromArgb(70, 130, 180); // SteelBlue

            return button;
        }

        
        public static FlowLayoutPanel CreateBottomButtons(EventHandler addClick, EventHandler editClick, EventHandler deleteClick, Size clientSize)
        {
            var panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Anchor = AnchorStyles.Bottom,
                Padding = new Padding(0, 10, 0, 10)
            };

            panel.Controls.Add(CreateButton("Добавить", addClick));
            panel.Controls.Add(CreateButton("Изменить", editClick));
            panel.Controls.Add(CreateButton("Удалить", deleteClick));

            int paddingLeftRight = (clientSize.Width - panel.PreferredSize.Width) / 2;
            panel.Margin = new Padding(paddingLeftRight, 0, paddingLeftRight, 0);

            return panel;
        }
        
        public static FlowLayoutPanel CreateRightButtons(EventHandler addClick, EventHandler editClick, EventHandler deleteClick, DataGridView dataGridView)
        {
            var panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                Anchor = AnchorStyles.Right,
                Padding = new Padding(0, 10, 0, 10)
            };

            panel.Controls.Add(CreateButton("Добавить", addClick));
            panel.Controls.Add(CreateButton("Изменить", editClick));
            panel.Controls.Add(CreateButton("Удалить", deleteClick));

            panel.Location = new Point(dataGridView.Right + 10, dataGridView.Top);

            return panel;
        }
        
        public static void AddRightPanelToForm(Form form, FlowLayoutPanel panel)
        {
            form.Controls.Add(panel);
            form.Resize += (sender, e) => 
            {
                panel.Location = new Point(form.ClientSize.Width - panel.Width - 10, panel.Location.Y);
            };
        }
        
        public static void AddPanelToForm(Form form, FlowLayoutPanel panel)
        {
            panel.Location = new Point((form.ClientSize.Width - panel.Width) / 2, form.ClientSize.Height - panel.Height - 10); 
            form.Controls.Add(panel);
            form.Resize += (sender, e) => 
            {
                panel.Location = new Point((form.ClientSize.Width - panel.Width) / 2, form.ClientSize.Height - panel.Height - 10);
            };
        }
        
    }
}