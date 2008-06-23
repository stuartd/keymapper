using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KeyMapper
{
    public partial class IdentifyNumLockKey : KMBaseForm
    {
        KeyMapper.KeySniffer _sniffer;
        bool _numLockIdentified;
        bool _numLockIsExtended;
        float _scale;

        public IdentifyNumLockKey()
        {
            InitializeComponent();
        }

        public IdentifyNumLockKey(bool disabling)
            : this()
        {

            lblExplanation.Text = "Keyboard manufacturers use different \ncodes to identify Num Lock" +
                "\n\nTo make sure Num Lock is " +
                (disabling ? "disabled" : "mapped correctly") + ",\n" +
                "please press Num Lock now:";
            _scale = ((float)AppController.DpiY / 96F);

            KeyPictureBox.SetImage(ButtonImages.GetButtonImage
                (-1, -1, BlankButton.Blank, 0, 0, _scale, ButtonEffect.None));

            StartCapture();

            FormClosing += IdentifyNumLockKeyFormClosing;

        }

        void IdentifyNumLockKeyFormClosing(object sender, FormClosingEventArgs e)
        {
            StopCapture();

            if (e.CloseReason != CloseReason.ApplicationExitCall
                && e.CloseReason != CloseReason.TaskManagerClosing
                && e.CloseReason != CloseReason.WindowsShutDown)
            {
                if (_numLockIdentified)
                {
                    AppController.SetNumLockExtendedStatus(_numLockIsExtended);
                }
            }
        }

        private void StartCapture()
        {

            if (_sniffer == null)
            {
                _sniffer = new KeySniffer(true);
                _sniffer.KeyPressed += OnKeyPress;
            }

            _sniffer.ActivateHook();

        }

        private void StopCapture()
        {
            _sniffer.DeactivateHook();
        }

        private void OnKeyPress(object sender, KeyMapperKeyPressedEventArgs e)
        {
            int scancode = e.Key.Scancode;
            int extended = e.Key.Extended;

            if (scancode == 69 && (extended == 0 || extended == 224))
            {
                KeyPictureBox.SetImage(ButtonImages.GetButtonImage
                        (BlankButton.Blank, _scale, "Num Lock", ButtonEffect.None));
                _numLockIdentified = true;
                _numLockIsExtended = (extended == 224);
                OKButton.Enabled = true;
            }
            else
            {
                KeyPictureBox.SetImage(ButtonImages.GetButtonImage
                        (scancode, extended, BlankButton.Blank, 0, 0, _scale, ButtonEffect.None));
                _numLockIdentified = false;
                OKButton.Enabled = false;

            }
        }

		private void OKButtonClick(object sender, EventArgs e)
		{
			this.Close();
		}

    }
}
