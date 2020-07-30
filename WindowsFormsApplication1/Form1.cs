using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GarronT.CMS.Model;
namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            long projectId = long.Parse(textBox1.Text);
            long installerId = long.Parse(textBox2.Text);

            DateTime dt = dateTimePicker1.Value;
            DateTime dt1 = dateTimePicker2.Value;

            CompassEntities objCompassEntities = new CompassEntities();

            var res = objCompassEntities.PROC_TestSP(projectId, installerId, dt, dt1);

            MessageBox.Show("operation completed successfully");
            //         var query = database.Posts    // your starting point - table in the "from" statement
            //.Join(database.Post_Metas, // the source table of the inner join
            //   post => post.ID,        // Select the primary key (the first part of the "on" clause in an sql "join" statement)
            //   meta => meta.Post_ID,   // Select the foreign key (the second part of the "on" clause)
            //   (post, meta) => new { Post = post, Meta = meta }) // selection
            //.Where(postAndMeta => postAndMeta.Post.ID == id);    // where statement



        }
    }
}
