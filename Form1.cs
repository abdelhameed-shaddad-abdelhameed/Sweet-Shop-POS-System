using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing; 
using System.IO;
using System.Windows.Forms;

namespace sweet
{
    public partial class Form1 : Form
    {
        List<SweetItem> availableSweets = new List<SweetItem>();
        decimal totalPrice = 0;

     
        string currentReceiptText = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetupForm();
        }

        private void SetupForm()
        {
            if (gridCart.Columns.Count == 0)
            {
                gridCart.ColumnCount = 2;
                gridCart.Columns[0].Name = "Item";
                gridCart.Columns[1].Name = "Price";
                gridCart.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }

            LoadMockData();
            RenderItems();
        }

        private void LoadMockData()
        {
           
            availableSweets.Add(new SweetItem { Name = "Chocolate Cake", Price = 300.00m, ImageFileName = "cake.jpg" });
            availableSweets.Add(new SweetItem { Name = "Donut", Price = 35.00m, ImageFileName = "donut.jpg" });
            availableSweets.Add(new SweetItem { Name = "Cheesecake", Price = 35.00m, ImageFileName = "cheese.jpg" });
            availableSweets.Add(new SweetItem { Name = "Macaron", Price = 20.00m, ImageFileName = "macaron.jpg" });
            availableSweets.Add(new SweetItem { Name = "Cupcake", Price = 25.00m, ImageFileName = "cupcake.jpg" });
        }

        private void RenderItems()
        {
            if (flowLayoutPanelItems.Controls.Count > 0)
                flowLayoutPanelItems.Controls.Clear();

            foreach (var sweet in availableSweets)
            {
                Panel pnl = new Panel();
                pnl.Size = new Size(150, 220);
                pnl.BorderStyle = BorderStyle.FixedSingle;
                pnl.Margin = new Padding(10);
                pnl.BackColor = Color.White;

                PictureBox pb = new PictureBox();
                pb.Size = new Size(130, 120);
                pb.Location = new Point(10, 10);
                pb.SizeMode = PictureBoxSizeMode.StretchImage;

                string fullPath = Path.Combine(Application.StartupPath, "Images", sweet.ImageFileName);
                if (File.Exists(fullPath))
                {
                    pb.Image = Image.FromFile(fullPath);
                }
                else
                {
                    pb.BackColor = Color.Pink;
                }

                Label lblName = new Label();
                lblName.Text = sweet.Name;
                lblName.Location = new Point(10, 140);
                lblName.AutoSize = true;
                lblName.Font = new Font("Arial", 10, FontStyle.Bold);

                Label lblPrice = new Label();
                lblPrice.Text = "جنيه" + sweet.Price.ToString("0.00");
                lblPrice.Location = new Point(10, 160);
                lblPrice.ForeColor = Color.Green;

                Button btnAdd = new Button();
                btnAdd.Text = "Add to Cart";
                btnAdd.Size = new Size(130, 30);
                btnAdd.Location = new Point(10, 185);
                btnAdd.BackColor = Color.Orange;
                btnAdd.FlatStyle = FlatStyle.Flat;

                btnAdd.Click += (s, args) => AddToCart(sweet);

                pnl.Controls.Add(pb);
                pnl.Controls.Add(lblName);
                pnl.Controls.Add(lblPrice);
                pnl.Controls.Add(btnAdd);

                flowLayoutPanelItems.Controls.Add(pnl);
            }
        }

        private void AddToCart(SweetItem item)
        {
            gridCart.Rows.Add(item.Name, item.Price);
            totalPrice += item.Price;
            lblTotal.Text = "Total: جنيه" + totalPrice.ToString("0.00");
        }

       

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            if (totalPrice == 0)
            {
                MessageBox.Show("السلة فارغة!", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

          
            PrepareReceipt();

          
            DialogResult result = MessageBox.Show(
                "تمت عملية الدفع بنجاح!\n\nهل تريد طباعة الفاتورة؟\n(اضغط Yes للطباعة، No للحفظ كملف نصي، Cancel للإلغاء)",
                "خيارات الفاتورة",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                PrintReceipt(); 
            }
            else if (result == DialogResult.No)
            {
                SaveReceiptToFile(); 
            }

          
            gridCart.Rows.Clear();
            totalPrice = 0;
            lblTotal.Text = "Total: $0.00";
        }

     
        private void PrepareReceipt()
        {
            currentReceiptText = "";
            currentReceiptText += "=========== SWEET SHOP ===========\n";
            currentReceiptText += $"Date: {DateTime.Now}\n";
            currentReceiptText += "==================================\n\n";
            currentReceiptText += "Items:\n";

            foreach (DataGridViewRow row in gridCart.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    string name = row.Cells[0].Value.ToString();
                    string price = row.Cells[1].Value.ToString();
                  
                    currentReceiptText += $"{name.PadRight(20)} \t ${price}\n";
                }
            }

            currentReceiptText += "\n==================================\n";
            currentReceiptText += $"TOTAL AMOUNT: \t\t ${totalPrice}\n";
            currentReceiptText += "==================================\n";
            currentReceiptText += "Thank you for visiting us!";
        }


        private void SaveReceiptToFile()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text File|*.txt";
            sfd.FileName = "Invoice_" + DateTime.Now.ToString("yyyyMMdd_HHmm");

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, currentReceiptText);
                MessageBox.Show("تم حفظ الفاتورة بنجاح!", "حفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

   
        private void PrintReceipt()
        {
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrintPage += new PrintPageEventHandler(PrintDoc_PrintPage);

            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDoc;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDoc.Print();
            }
        }

     
        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
      
            Font printFont = new Font("Courier New", 12);
            e.Graphics.DrawString(currentReceiptText, printFont, Brushes.Black, 10, 10);
        }
    }

    public class SweetItem
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImageFileName { get; set; }
    }
}