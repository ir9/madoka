using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testguid00
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			/*
			dataGridView1.Rows.Add(;
			dataGridView1.BindingContext

			DataGridViewRow row;
			row.Cells.Add(;
			DataGridViewCell cell;
			DataGridViewTextBoxCell
			*/
			dataGridView1.AutoGenerateColumns = false;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
			col.HeaderText = "ID";
			col.DataPropertyName = "ID";
			dataGridView1.Columns.Add(col);
			col = new DataGridViewTextBoxColumn();
			col.HeaderText = "Value";
			col.DataPropertyName = "Value";
			dataGridView1.Columns.Add(col);

			DataTable table = new DataTable();
			table.Columns.Add("ID");
			table.Columns.Add("Value");

			var it = Enumerable.Range(0, 2000)
				.Select((num) => table.Rows.Add(num, $"{num} かしら"));
			it.All((num) => true);

			bindingSource1.RaiseListChangedEvents = false;
			bindingSource1.SuspendBinding();
			bindingSource1.DataSource = table;
			bindingSource1.RaiseListChangedEvents = true;
			bindingSource1.ResumeBinding();
			bindingSource1.ResetBindings(true);
		}


		private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{

		}
	}

	class Record
	{
		public int ID { get; set; }
		public string Value { get; set; }
	}

}
