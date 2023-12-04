using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GDMultiStash.Forms
{
    internal partial class ColorsDialogForm : DialogForm
    {

        public ColorsDialogForm() : base()
        {
            InitializeComponent();
        }

        protected override void Localize(Global.LocalizationManager.StringsHolder L)
        {
            //Text = L.AboutButton();
            saveButton.Text = L.SaveButton();
        }

        private void ColorsDialogForm_Load(object sender, EventArgs e)
        {

        }

        private void ColorsDialogForm_Shown(object sender, EventArgs e)
        {
            TopMost = false;
        }

    }
}
