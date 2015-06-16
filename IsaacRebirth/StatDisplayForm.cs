using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IsaacRebirth
{
    public partial class StatDisplayForm : Form
    {
        public int _redHearts;
        public int _soulHearts;
        public int _blackHearts;
        public double _dmgTotal;
        public double _dmgBase = 0;
        public double _dmgMultiplier;
        public double _tears;
        public double _shotSpeed;
        public double _range;
        public double _speed;
        public int _luck;
        public int _bombs;
        public int _keys;
        public int _coins;
        public int _hasFlight;
        public int _isLotf;
        public int _isGuppy;
        public double _delay;
        public int _delayFlag = 0;

        public List<string> _pills = new List<string>
        {
            "Balls of Steel","Bombs are Key","Health Down","Health Up","Luck Down","Luck Up","Range Down","Range Up",
            "Speed Down","Speed Up","Tears Down","Tears Up"
        };

        //CharacterSelectionForm characterSelectionForm = new CharacterSelectionForm();
        MySqlConnection myConnection = new MySqlConnection("Server=localhost; Database=isaac_rebirth; Uid=root; Pwd=root;");

        public StatDisplayForm()
        {
            InitializeComponent();
        }

        // after an item pool is selected, populate the item list combobox for selection
        private void PoolSelectedPopulateItemlist(object sender, EventArgs e)
        {
            ComboBox poolSender = sender as ComboBox;
            comboBoxItems.Items.Clear();

            try
            {
                myConnection.Open();
                MySqlCommand pcmd = new MySqlCommand("SELECT j_itemName FROM isaac_rebirth.item_junction WHERE j_itemPool = @itemPool ORDER BY j_itemName ASC", myConnection);
                pcmd.Parameters.AddWithValue("@itemPool", poolSender.Text);
                MySqlDataReader reader = pcmd.ExecuteReader();
                while (reader.Read())
                {
                    comboBoxItems.Items.Add(reader.GetString("j_itemName"));
                }
                pcmd.Parameters.Clear();
            }
            catch (Exception ea)
            {
                myConnection.Close();
                MessageBox.Show(ea.ToString(), "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            myConnection.Close();
        }

        // after clicking the Add Item Button calculate damage and update the item list
        private void AddItem(object sender, EventArgs e)
        {
            string itemName = comboBoxItems.Text;

            try
            {
                myConnection.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM isaac_rebirth.items WHERE itemName = @iName", myConnection);
                cmd.Parameters.AddWithValue("@iName", itemName);
                MySqlDataReader reader = cmd.ExecuteReader();

                if(reader.Read())
                {
                    UpdateStats(reader);
                    SpecialItems(itemName);
                    CalculateDamage();
                    ValidateData();
                    DisplayItem(itemName);
                    DisplayStats();
                }

                cmd.Parameters.Clear();

            }
            catch (Exception ea)
            {
                myConnection.Close();
                MessageBox.Show(ea.ToString(), "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            myConnection.Close();
        }

        // calculate the fields values
        public void UpdateStats(MySqlDataReader reader)
        {
            _bombs += reader.GetInt32("itemBombs");
            _keys += reader.GetInt32("itemKeys");
            _coins += reader.GetInt32("itemCoins");
            _redHearts += reader.GetInt32("itemRedHeart");
            _soulHearts += reader.GetInt32("itemSoulHeart");
            _blackHearts += reader.GetInt32("itemBlackHeart");
            _hasFlight += reader.GetInt32("itemHasFlight");
            _dmgBase += reader.GetDouble("itemDmgBase");
            _dmgMultiplier += reader.GetDouble("itemDmgMulti");
            _tears += reader.GetDouble("itemTears");
            _shotSpeed += reader.GetDouble("itemShotSpeed");
            _range += reader.GetDouble("itemRange");
            _speed += reader.GetDouble("itemSpeed");
            _luck += reader.GetInt32("itemLuck");
            _isGuppy += reader.GetInt32("itemIsGuppy");
            _isLotf += reader.GetInt32("itemIsLotf");
        }

        // handle any special item cases
        public void SpecialItems(string itemName)
        {
            if (itemName == "20/20")
            {
                _dmgMultiplier = _dmgMultiplier * 2;
            }
            if (itemName == "Abaddon")
            {
                _redHearts = 0;
                _blackHearts += 6;
            }
            if (itemName == "Blue Cap")
            {
                _shotSpeed = _shotSpeed * .84;
            }
            if (itemName == "Cat-O-Nine-Tails")
            {
                _shotSpeed = _shotSpeed * 1.23;
            }
            if (itemName == "Dead Cat")
            {
                _redHearts = 1;
            }
            if (itemName == "Dr. Fetus")
            {
                _dmgBase = _dmgBase * 3;
            }
            if (itemName == "Epic Fetus")
            {
                _dmgBase = _dmgBase * 20;
            }
            if (itemName == "Eve's Mascara")
            {
                _tears = _tears * .5;
                _dmgBase = _dmgBase * 2;
            }
            if (itemName == "Godhead")
            {
                _shotSpeed = _shotSpeed * .70;
            }
            if (itemName == "The Inner Eye")
            {
                _dmgMultiplier = _dmgMultiplier * 3;
                _delayFlag = 1;
            }
            if (itemName == "Libra")
            {
                double balance = (_dmgBase + _tears + _speed + _range/4) / 4;
                _dmgBase = balance;
                _tears = balance;
                _speed = balance;
                _range = balance;
            }
            if (itemName == "Magic Mushroom")
            {
                _dmgMultiplier = _dmgMultiplier *1.5;
            }
            if (itemName == "Mutant Spider")
            {
                _dmgMultiplier = _dmgMultiplier * 4;
                _delayFlag = 1; 
            }
            if (itemName == "Polyphemus")
            {
                _dmgBase = (_dmgBase + 4) * 2;
                _delayFlag = 1; 
            }
            if (itemName == "Sacred Heart")
            {
                _dmgBase = (_dmgBase * 2.3) + 1;
                _shotSpeed = _shotSpeed * .75;
            }
            if (itemName == "Soy Milk")
            {
                _dmgMultiplier = _dmgMultiplier * .2;
                _delayFlag = 2;
            }
            if (itemName == "Technology 2")
            {
                _dmgMultiplier = _dmgMultiplier * .65;
            }
            if (itemName == "Toothpicks")
            {
                _shotSpeed = _shotSpeed * 1.16;
            }
        }

        // calculate the damage per second
        public void CalculateDamage()
        {
            CalculateDelay();
            double fireRate = 30 / _delay;
            if (fireRate > 6)
            {
                fireRate = 6;
            }
            double dmg = 3.5 * Math.Sqrt(1 + _dmgBase * 1.2);
            _dmgTotal = dmg * _dmgMultiplier * fireRate;
        }

        // calculate the delay between shots
        private void CalculateDelay()
        {

            if (_tears > 0)
            {
                _delay = 16 - 6 * Math.Sqrt(1 + 1.3 * _tears);
            }
            else if (_tears == 0)
            {
                _delay = 10;
            }
            else if (_tears > -.77 && _tears < 0)
            {
                _delay = 16 - 6 * Math.Sqrt(1 + 1.3 * _tears) - _tears * 6;
            }
            else
            {
                _delay = 16 - _tears * 6;
            }

            if (_delayFlag == 1)
            {
                _delay = (_delay * 2.1) + 3;
            }
            else if (_delayFlag == 2)
            {
                _delay = (_delay / 4) - 2;
            }

            if (_delay < 5)
            {
                _delay = 5;
            }
        }

        // validate data so that it doesn't exceed maximum values
        private void ValidateData()
        {
            if (_keys > 99)
            {
                _keys = 99;
            }
            if (_bombs > 99)
            {
                _bombs = 99;
            }
            if (_coins > 99)
            {
                _coins = 99;
            }
            if (_redHearts > 12)
            {
                _redHearts = 12;
            }
            if (_speed < .01)
            {
                _speed = .01;
            }
            if (_speed > 2)
            {
                _speed = 2;
            }
        }

        // update the lables with the correct values
        public void DisplayStats()
        {
            label_coin.Text = _coins.ToString();
            label_bomb.Text = _bombs.ToString();
            label_key.Text = _keys.ToString();
            label_redHeart.Text = _redHearts.ToString();
            label_soulHeart.Text = _soulHearts.ToString();
            label_blackHeart.Text = _blackHearts.ToString();
            label_dmgBase.Text = _dmgBase.ToString();
            label_dmgMulti.Text = _dmgMultiplier.ToString();
            label_tears.Text = _tears.ToString();
            label_range.Text = _range.ToString();
            label_shotSpeed.Text = _shotSpeed.ToString();
            label_speed.Text = _speed.ToString();
            label_luck.Text = _luck.ToString();
            label_dmgTotal.Text = _dmgTotal.ToString();

            if (_isLotf >= 3)
            {
                checkBox_lotf.Checked = true;
            }

            if (_hasFlight >= 1)
            {
                checkBox_flight.Checked = true;
            }

            if (_isGuppy >= 3)
            {
                checkBox_guppy.Checked = true;
            }
        }

        public void DisplayItem(string itemName)
        {
            listViewItems.Items.Add(itemName);
        }

        private void buttonAddPill_Click(object sender, EventArgs e)
        {
            string pillEffect = comboBoxPills.Text;

            CalculatePillStats(pillEffect);
            CalculateDamage();
            ValidateData();
            DisplayStats();
        }

        private void CalculatePillStats(string pillEffect)
        {
            switch (pillEffect)
            {
                case "Balls of Steel":
                    _soulHearts += 2;
                    break;
                case "Bombs are Key":
                    int tmp = _bombs;
                    _bombs = _keys;
                    _keys = tmp;
                    break;
                case "Health Down":
                    _redHearts -= 1;
                    break;
                case "Health Up":
                    _redHearts += 1;
                    break;
                case "Luck Down":
                    _luck -= 1;
                    break;
                case "Luck Up":
                    _luck += 1;
                    break;
                case "Range Down":
                    _range -= 2;
                    break;
                case "Range Up":
                    _range += 2.5;
                    break;
                case "Speed Down":
                    _speed -= .12;
                    break;
                case "Speed Up":
                    _speed += .15;
                    break;
                case "Tears Down":
                    _tears -= .28;
                    break;
                case "Tears Up":
                    _tears += .35;
                    break;
            }
        }

        private void buttonNewRun_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

    }
}
