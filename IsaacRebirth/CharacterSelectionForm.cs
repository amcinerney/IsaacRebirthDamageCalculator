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
    public partial class CharacterSelectionForm : Form
    {
        public string _startingItem1;
        public string _startingItem2;

        //initiates the stat display form to be used throughout the application
        StatDisplayForm statDisplayForm = new StatDisplayForm();

        MySqlConnection myConnection = new MySqlConnection("Server=localhost; Database=isaac_rebirth; Uid=root; Pwd=root;");



        public CharacterSelectionForm()
        {
            InitializeComponent();
        }

        // query the database once a character is selected and populate the apropriate variables
        private void CreateVariables(string cName)
        {
            try
            {
                myConnection.Open();
                //query the starting_item table for starting items
                MySqlCommand startingInfoCommand = new MySqlCommand("SELECT * FROM isaac_rebirth.characters " + 
                                                                     "JOIN isaac_rebirth.starting_items " +
                                                                     "WHERE characters.name = '"+cName+"' " +
                                                                     "AND starting_items.character_name = '"+cName+"'", myConnection);

                MySqlDataReader startingInfoReader = startingInfoCommand.ExecuteReader();
                if(startingInfoReader.Read())
                {
                    statDisplayForm._bombs = startingInfoReader.GetInt32("bombs");
                    statDisplayForm._keys = startingInfoReader.GetInt32("keys");
                    statDisplayForm._coins = startingInfoReader.GetInt32("coins");
                    statDisplayForm._redHearts = startingInfoReader.GetInt32("red_hearts");
                    statDisplayForm._soulHearts = startingInfoReader.GetInt32("soul_hearts");
                    statDisplayForm._blackHearts = startingInfoReader.GetInt32("black_hearts");
                    statDisplayForm._hasFlight = startingInfoReader.GetInt32("has_flight");
                    statDisplayForm._dmgMultiplier = startingInfoReader.GetDouble("dmg_multiplier");
                    statDisplayForm._tears = startingInfoReader.GetDouble("tears");
                    statDisplayForm._shotSpeed = startingInfoReader.GetDouble("shot_speed");
                    statDisplayForm._range = startingInfoReader.GetDouble("range");
                    statDisplayForm._speed = startingInfoReader.GetDouble("speed");
                    statDisplayForm._luck = startingInfoReader.GetInt32("luck");

                    // check to see if the starting item columns are null. 
                    // if false then populate the item list
                    var infocheck1 = startingInfoReader.GetOrdinal("starting_item1");
                    if (startingInfoReader.IsDBNull(infocheck1) == false)
                    {
                        _startingItem1 = startingInfoReader.GetString("starting_item1");
                        statDisplayForm.DisplayItem(_startingItem1);
                    }

                    var infocheck2 = startingInfoReader.GetOrdinal("starting_item2");
                    if (startingInfoReader.IsDBNull(infocheck2) == false)
                    {
                        _startingItem1 = startingInfoReader.GetString("starting_item2");
                        statDisplayForm.DisplayItem(_startingItem1);
                    }

                }
                myConnection.Close();
            }

            catch(Exception e)
            {
                myConnection.Close();
                MessageBox.Show(e.ToString(),"Database Connection Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        // populate the stat form with the characters base stats and starting items
        private void CreateCharacter()
        {
            statDisplayForm.CalculateDamage();
            statDisplayForm.DisplayStats();

        }

        // populate the combobox with the item pools
        private void PopulatePoolDropdown()
        {

            try
            {
                myConnection.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT pool_name FROM isaac_rebirth.item_pools ORDER BY pool_name ASC", myConnection);

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    statDisplayForm.comboBoxPools.Items.Add(reader.GetString("pool_name"));
                }
                myConnection.Close();
            }

            catch (Exception e)
            {
                myConnection.Close();
                MessageBox.Show(e.ToString(), "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void PopulatePillsDropdown()
        {
            foreach (string item in statDisplayForm._pills)
            {
                statDisplayForm.comboBoxPills.Items.Add(item);
            }
        }

        //after selecting a character the form view changes to the character calculation sheet
        private void StartNewRun(object sender, EventArgs e)
        {
            Label character = sender as Label;

            if (character != null)
            {
                //execute the creation of the character screen
                //Determine which character was selected
                CreateVariables(character.Text);
                CreateCharacter();
                PopulatePoolDropdown();
                PopulatePillsDropdown();
                if (character.Text == "Isaac")
                {
                    statDisplayForm.pictureBox1.Image = Image.FromFile("../../Images/Characters/Isaac_bigger.png");
                }
                if (character.Text == "Magdalene")
                {
                    statDisplayForm.pictureBox1.Image = Image.FromFile("../../Images/Characters/Magdalene_bigger.png");
                }
                if (character.Text == "Cain")
                {
                    statDisplayForm.pictureBox1.Image = Image.FromFile("../../Images/Characters/Cain_bigger.png");
                }
                if (character.Text == "Judas")
                {
                    statDisplayForm.pictureBox1.Image = Image.FromFile("../../Images/Characters/Judas_bigger.png");
                }
                if (character.Text == "???")
                {
                    statDisplayForm.pictureBox1.Image = Image.FromFile("../../Images/Characters/Blue_Baby_bigger.png");
                }
                if (character.Text == "Eve")
                {
                    statDisplayForm.pictureBox1.Image = Image.FromFile("../../Images/Characters/Eve_bigger.png");
                }
                if (character.Text == "Samson")
                {
                    statDisplayForm.pictureBox1.Image = Image.FromFile("../../Images/Characters/Samson_bigger.png");
                }
                if (character.Text == "Azazel")
                {
                    statDisplayForm.pictureBox1.Image = Image.FromFile("../../Images/Characters/Azazel_bigger.png");
                }
                if (character.Text == "Lazarus")
                {
                    statDisplayForm.pictureBox1.Image = Image.FromFile("../../Images/Characters/Lazarus_bigger.png");
                }
                if (character.Text == "The Lost")
                {
                    statDisplayForm.pictureBox1.Image = Image.FromFile("../../Images/Characters/The_Lost_bigger.png");
                }

                //hide the character selection form and display the character stat form. 
                this.Hide();
                statDisplayForm.FormClosed += new FormClosedEventHandler(statDisplayForm_FormClosed);
                statDisplayForm.ShowDialog();
            }
        }

        //called when the stat display form is closed exiting the application
        private void statDisplayForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
        }
    }
}
