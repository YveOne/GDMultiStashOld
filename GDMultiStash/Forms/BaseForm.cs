using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GDMultiStash.Forms
{
    internal class BaseForm : Form
    {

        public BaseForm() : base()
        {
            Load += delegate {
                Utils.Funcs.RunThread(1, () => {
                    Initialize();
                    Localize();
                });
            };
        }

        protected virtual void Initialize()
        {
        }

        protected virtual void Localize(Global.LocalizationManager.StringsHolder L)
        {
        }

        public void Localize()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => { Localize(); }));
                return;
            }
            Localize(G.L);
        }

    }
}
