﻿using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using TweeFlyPro;

namespace TweeFly
{
    public partial class Form1 : Form
    {

        private ColumnHeader SortingColumn = null;

        public Configuration conf = new Configuration(true);
        public string projectName = "";
        private string projectDir = "";
        //private string imgDir = "img";
        private const string APP_TITLE = "TweeFly - Interactive Story Setup for SugarCube2";

        public Form1()
        {
            InitializeComponent();
        }

        public void updateFromConf(Configuration _conf)
        {
            if (_conf != null)
            {
                checkBox1.Checked = _conf.inventoryActive;
                checkBox2.Checked = _conf.clothingActive;
                checkBox3.Checked = _conf.statsActive;
                checkBox4.Checked = _conf.daytimeActive;
                checkBox5.Checked = _conf.shopActive;
                checkBox6.Checked = _conf.moneyActive;
                checkBox12.Checked = _conf.jobsActive;
                checkBox19.Checked = _conf.charactersActive;

                // Build
                textBox47.Text = _conf.storyName;

                // Captions
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                dataGridView1.Columns.Add("captionName", "captionName");
                dataGridView1.Columns.Add("caption", "caption");
                for (int i=0; i<_conf.captions.Count; i++)
                {
                    var index = dataGridView1.Rows.Add();
                    dataGridView1.Rows[index].Cells["captionName"].Value = _conf.captions[i].captionName;
                    dataGridView1.Rows[index].Cells["caption"].Value = _conf.captions[i].caption;
                }
                dataGridView1.Columns[0].ReadOnly = true;
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                // Config
                checkBox40.Checked = _conf.navigationArrows;
                checkBox41.Checked = _conf.debugMode;

                checkBox43.Checked = _conf.resizeImagesInSidebar;
                numericUpDown26.Value = _conf.imageWidthInSidebar;
                numericUpDown27.Value = _conf.imageHeightInSidebar;

                checkBox42.Checked = _conf.resizeImagesInParagraph;
                numericUpDown29.Value = _conf.imageWidthInParagraph;
                numericUpDown28.Value = _conf.imageHeightInParagraph;

                checkBox44.Checked = _conf.resizeImagesInDialogs;
                numericUpDown31.Value = _conf.imageWidthInDialogs;
                numericUpDown24.Value = _conf.imageHeightInDialogs;

                numericUpDown30.Value = _conf.paragraphWidth;

                checkBox47.Checked = _conf.storeProjectFileAsBinary;
                checkBox46.Checked = _conf.listVariables;

                // Inventory
                checkBox9.Checked = _conf.inventoryLinkInSidebar;
                checkBox20.Checked = _conf.inventoryInSidebar;
                checkBox45.Checked = _conf.inventorySidebarTooltip;

                listView1.Items.Clear();
                for(int i=0; i<_conf.items.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = _conf.items[i].ID.ToString();
                    item.SubItems.Add(_conf.items[i].name);
                    item.SubItems.Add(_conf.items[i].description);
                    item.SubItems.Add(_conf.items[i].category);
                    item.SubItems.Add(_conf.items[i].shopCategory);
                    item.SubItems.Add(_conf.items[i].owned.ToString());
                    item.SubItems.Add(_conf.items[i].canBeBought.ToString());
                    item.SubItems.Add(_conf.items[i].buyPrice.ToString());
                    item.SubItems.Add(_conf.items[i].sellPrice.ToString());
                    item.SubItems.Add(_conf.items[i].canOwnMultiple.ToString());
                    item.SubItems.Add(_conf.items[i].image);
                    item.SubItems.Add(_conf.items[i].skill1);
                    item.SubItems.Add(_conf.items[i].skill2);
                    item.SubItems.Add(_conf.items[i].skill3);
                    item.SubItems.Add(_conf.items[i].passage);
                    listView1.Items.Add(item);
                }
                checkBox25.Checked = _conf.inventoryUseSkill1;
                checkBox26.Checked = _conf.inventoryUseSkill2;
                checkBox27.Checked = _conf.inventoryUseSkill3;

                for(int i=0; i<checkedListBox3.Items.Count; i++)
                {
                    checkedListBox3.SetItemChecked(i, false);
                }
                for(int i=0; i<_conf.displayInInventory.Count; i++)
                {
                    for(int j=0; j<checkedListBox3.Items.Count; j++)
                    {
                        if (checkedListBox3.Items[j].ToString().ToUpper().Equals(_conf.displayInInventory[i].ToUpper()))
                        {
                            checkedListBox3.SetItemChecked(j, true);
                        }
                    }
                }

                // Reset rest
                numericUpDown3.Value = 0;
                numericUpDown5.Value = 0;
                numericUpDown22.Value = 0;
                numericUpDown4.Value = 0;
                textBox7.Text = "";
                textBox30.Text = "";
                comboBox3.Text = "";
                comboBox3.Items.Clear();
                textBox8.Text = "";
                textBox10.Text = "";
                textBox29.Text = "";
                textBox2.Text = "";
                textBox28.Text = "";


                // Wardrobe
                checkBox8.Checked = _conf.clothingLinkInSidebar;
                checkBox21.Checked = _conf.clothingInSidebar;
                checkBox34.Checked = _conf.wardrobeLinkInSidebar;
                
                listView2.Items.Clear();
                for(int i=0; i< _conf.clothing.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = _conf.clothing[i].ID.ToString();
                    item.SubItems.Add(_conf.clothing[i].name);
                    item.SubItems.Add(_conf.clothing[i].description);
                    item.SubItems.Add(_conf.clothing[i].category);
                    item.SubItems.Add(_conf.clothing[i].shopCategory);
                    item.SubItems.Add(_conf.clothing[i].bodyPart);
                    item.SubItems.Add(_conf.clothing[i].owned.ToString());
                    item.SubItems.Add(_conf.clothing[i].isWornAtBeginning.ToString());
                    item.SubItems.Add(_conf.clothing[i].canBeBought.ToString());
                    item.SubItems.Add(_conf.clothing[i].buyPrice.ToString());
                    item.SubItems.Add(_conf.clothing[i].sellPrice.ToString());
                    item.SubItems.Add(_conf.clothing[i].canOwnMultiple.ToString());
                    item.SubItems.Add(_conf.clothing[i].image);
                    item.SubItems.Add(_conf.clothing[i].skill1);
                    item.SubItems.Add(_conf.clothing[i].skill2);
                    item.SubItems.Add(_conf.clothing[i].skill3);
                    listView2.Items.Add(item);
                }
                checkBox28.Checked = _conf.clothingUseSkill1;
                checkBox29.Checked = _conf.clothingUseSkill2;
                checkBox30.Checked = _conf.clothingUseSkill3;

                for (int i = 0; i < checkedListBox4.Items.Count; i++)
                {
                    checkedListBox4.SetItemChecked(i, false);
                }
                for (int i = 0; i < _conf.displayInWardrobe.Count; i++)
                {
                    for (int j = 0; j < checkedListBox4.Items.Count; j++)
                    {
                        if (checkedListBox4.Items[j].ToString().ToUpper().Equals(_conf.displayInWardrobe[i].ToUpper()))
                        {
                            checkedListBox4.SetItemChecked(j, true);
                        }
                    }
                }

                for (int i = 0; i < checkedListBox2.Items.Count; i++)
                {
                    checkedListBox2.SetItemChecked(i, false);
                }
                for (int i = 0; i < _conf.displayInClothingView.Count; i++)
                {
                    for (int j = 0; j < checkedListBox2.Items.Count; j++)
                    {
                        if (checkedListBox2.Items[j].ToString().ToUpper().Equals(_conf.displayInClothingView[i].ToUpper()))
                        {
                            checkedListBox2.SetItemChecked(j, true);
                        }
                    }
                }

                // Reset rest
                numericUpDown6.Value = 0;
                numericUpDown7.Value = 0;
                numericUpDown8.Value = 0;
                textBox11.Text = "";
                comboBox2.Items.Clear();
                textBox13.Text = "";
                textBox14.Text = "";
                checkBox15.Checked = false;
                checkBox16.Checked = false;
                textBox20.Text = "";
                textBox23.Text = "";
                textBox24.Text = "";

                // Stats
                checkBox7.Checked = _conf.statsLinkInSidebar;
                checkBox23.Checked = _conf.statsInSidebar;
                listView5.Items.Clear();
                for(int i=0; i< _conf.stats.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = _conf.stats[i].ID.ToString();
                    item.SubItems.Add(_conf.stats[i].name);
                    item.SubItems.Add(_conf.stats[i].value);
                    item.SubItems.Add(_conf.stats[i].unit);
                    item.SubItems.Add(_conf.stats[i].image);
                    item.SubItems.Add(_conf.stats[i].description);
                    item.SubItems.Add(_conf.stats[i].visible.ToString());
                    item.SubItems.Add(_conf.stats[i].isSkill);
                    listView5.Items.Add(item);
                }

                // Reset rest
                textBox16.Text = "";
                textBox18.Text = "";
                textBox19.Text = "";
                numericUpDown12.Value = 0;

                // Daytime
                checkBox10.Checked = _conf.daytimeInSidebar;
                switch(_conf.daytimeFormat)
                {
                    case 0:
                        radioButton1.Checked = true;
                        break;
                    case 1:
                        radioButton2.Checked = true;
                        break;
                    case 2:
                        radioButton3.Checked = true;
                        break;
                    case 3:
                        radioButton4.Checked = true;
                        break;
                }
                dateTimePicker3.Value = conf.startDate;

                // Shops
                listView3.Items.Clear();
                for(int i=0; i<_conf.shops.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = _conf.shops[i].ID.ToString();
                    item.SubItems.Add(_conf.shops[i].name);
                    item.SubItems.Add(_conf.shops[i].opening.ToString("HH:mm:ss"));
                    item.SubItems.Add(_conf.shops[i].closing.ToString("HH:mm:ss"));
                    listView3.Items.Add(item);
                }

                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);
                }
                for (int i = 0; i < _conf.itemPropertiesInShops.Count; i++)
                {
                    for (int j = 0; j < checkedListBox1.Items.Count; j++)
                    {
                        if (checkedListBox1.Items[j].ToString().ToUpper().Equals(_conf.itemPropertiesInShops[i].ToUpper()))
                        {
                            checkedListBox1.SetItemChecked(j, true);
                        }
                    }
                }

                checkBox39.Checked = _conf.shopUseSkill1;
                checkBox38.Checked = _conf.shopUseSkill2;
                checkBox37.Checked = _conf.shopUseSkill3;

                // Reset rest
                listView6.Items.Clear();
                numericUpDown10.Value = 0;
                textBox17.Text = "";
                comboBox1.Items.Clear();
                numericUpDown11.Value = 0;

                // Money
                numericUpDown1.Value = _conf.startMoney;
                numericUpDown2.Value = _conf.moneyPerDay;
                checkBox11.Checked = _conf.moneyInSidebar;

                // Jobs
                listView4.Items.Clear();
                for(int i=0; i<_conf.jobs.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = _conf.jobs[i].ID.ToString();
                    item.SubItems.Add(_conf.jobs[i].name);
                    item.SubItems.Add(_conf.jobs[i].description);
                    item.SubItems.Add(_conf.jobs[i].category);
                    item.SubItems.Add(_conf.jobs[i].available.ToString());
                    item.SubItems.Add(_conf.jobs[i].rewardMoney.ToString());
                    item.SubItems.Add(_conf.jobs[i].cooldown.ToString());
                    item.SubItems.Add(_conf.jobs[i].duration.ToString());
                    item.SubItems.Add(_conf.jobs[i].image);
                    item.SubItems.Add(_conf.jobs[i].passage);
                    listView4.Items.Add(item);
                }

                // Reset rest
                numericUpDown9.Value = 0;
                numericUpDown14.Value = 0;
                numericUpDown15.Value = 0;
                textBox15.Text = "";
                textBox21.Text = "";
                textBox22.Text = "";
                checkBox18.Checked = false;

                // Characters
                listView7.Items.Clear();
                for(int i=0; i<_conf.characters.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = _conf.characters[i].ID.ToString();
                    item.SubItems.Add(_conf.characters[i].name);
                    item.SubItems.Add(_conf.characters[i].category);
                    item.SubItems.Add(_conf.characters[i].description);
                    item.SubItems.Add(_conf.characters[i].known.ToString());
                    item.SubItems.Add(_conf.characters[i].age.ToString());
                    item.SubItems.Add(_conf.characters[i].gender);
                    item.SubItems.Add(_conf.characters[i].job);
                    item.SubItems.Add(_conf.characters[i].relation.ToString());
                    item.SubItems.Add(_conf.characters[i].image);
                    item.SubItems.Add(_conf.characters[i].color);
                    item.SubItems.Add(_conf.characters[i].skill1);
                    item.SubItems.Add(_conf.characters[i].skill2);
                    item.SubItems.Add(_conf.characters[i].skill3);
                    listView7.Items.Add(item);
                }
                checkBox36.Checked = _conf.charactersInSidebar;
                checkBox22.Checked = _conf.charactersLinkInSidebar;
                checkBox48.Checked = _conf.charactersSidebarTooltip;

                checkBox33.Checked = _conf.characterUseSkill1;
                checkBox32.Checked = _conf.characterUseSkill2;
                checkBox31.Checked = _conf.characterUseSkill3;

                for (int i = 0; i < checkedListBox5.Items.Count; i++)
                {
                    checkedListBox5.SetItemChecked(i, false);
                }
                for (int i = 0; i < _conf.displayInCharactersView.Count; i++)
                {
                    for (int j = 0; j < checkedListBox5.Items.Count; j++)
                    {
                        if (checkedListBox5.Items[j].ToString().ToUpper().Equals(_conf.displayInCharactersView[i].ToUpper()))
                        {
                            checkedListBox5.SetItemChecked(j, true);
                        }
                    }
                }

                // Reset rest
                numericUpDown16.Value = 0;
                numericUpDown17.Value = 0;
                numericUpDown18.Value = 0;
                textBox40.Text = "";
                textBox36.Text = "";
                textBox38.Text = "";
                textBox39.Text = "";
                textBox37.Text = "";
                textBox35.Text = "";
                textBox34.Text = "";
                textBox46.Text = "";
                textBox45.Text = "";


                // Update comboboxes in GUI
                // Shop categories in inventory
                comboBox3.Items.Clear();
                for (int i = 0; i < conf.shops.Count; i++)
                {
                    comboBox3.Items.Add(conf.shops[i].name);
                }

                // Shop categories in clothing
                comboBox2.Items.Clear();
                for (int i = 0; i < conf.shops.Count; i++)
                {
                    comboBox2.Items.Add(conf.shops[i].name);
                }

                // Job reward item IDs
                comboBox7.Text = "ITEM";
                comboBox4.Items.Clear();
                for (int i = 0; i < conf.items.Count; i++)
                {
                    comboBox4.Items.Add(conf.items[i].ID);
                }

                // Items in Shops
                comboBox1.Items.Clear();
                comboBox6.Text = "ITEM";
                for (int i = 0; i < conf.items.Count; i++)
                {
                    comboBox1.Items.Add(conf.items[i].ID);
                }

            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            conf.inventoryActive = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            conf.clothingActive = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            conf.statsActive = checkBox3.Checked;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            conf.daytimeActive = checkBox4.Checked;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            conf.shopActive = checkBox5.Checked;
            if (conf.shopActive && (!conf.moneyActive || !conf.inventoryActive))
            {
                MessageBox.Show("Shop system requires money and inventory system.");
                checkBox6.Checked = true;
                checkBox1.Checked = true;
                conf.moneyActive = true;
                conf.inventoryActive = true;
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            conf.moneyActive = checkBox6.Checked;
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            conf.jobsActive = checkBox12.Checked;
        }

        private void checkBox19_CheckedChanged(object sender, EventArgs e)
        {
            conf.charactersActive = checkBox19.Checked;
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            conf.inventoryLinkInSidebar = checkBox9.Checked;
        }

        /*
         * Inventory
         */

        // Add new item to inventory
        private void button3_Click(object sender, EventArgs e)
        {
            bool found = false;
            for(int i=0; i<conf.items.Count; i++)
            {
                if (conf.items[i].ID == numericUpDown3.Value)
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                MessageBox.Show("The ID " + numericUpDown3.Value + " is already set.");
            } else
            {
                Item item = new Item();
                item.ID = Convert.ToInt32(numericUpDown3.Value);
                item.name = textBox7.Text;
                item.description = textBox2.Text;
                item.canBeBought = checkBox13.Checked;
                item.skill1 = textBox30.Text;
                item.skill2 = textBox29.Text;
                item.skill3 = textBox28.Text;
                item.shopCategory = comboBox3.Text;
                item.category = textBox8.Text;
                item.image = textBox10.Text;
                item.buyPrice = Convert.ToInt32(numericUpDown5.Value);
                item.sellPrice = Convert.ToInt32(numericUpDown22.Value);
                item.canOwnMultiple = checkBox14.Checked;
                item.owned = Convert.ToInt32(numericUpDown4.Value);
                item.passage = textBox5.Text;

                // Copy file
                if (!Directory.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.INVENTORY_PATH_PREFIX))) {
                    Directory.CreateDirectory(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.INVENTORY_PATH_PREFIX));
                }
                if (File.Exists(textBox10.Text))
                {
                    File.Copy(textBox10.Text, Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.INVENTORY_PATH_PREFIX, Path.GetFileName(textBox10.Text)));
                }
                item.image = Path.GetFileName(textBox10.Text);
                textBox10.Text = Path.GetFileName(textBox10.Text);

                conf.items.Add(item);

                // Update inventory                
                updateInventory();

                numericUpDown3.Value += 1;
            }
        }

        // Update all items in the inventory list view
        private void updateInventory()
        {              
            listView1.Items.Clear();
            for (int i = 0; i < conf.items.Count; i++)
            {
                ListViewItem updateItem = new ListViewItem();
                updateItem.Text = conf.items[i].ID.ToString();
                updateItem.SubItems.Add(conf.items[i].name);
                updateItem.SubItems.Add(conf.items[i].description);
                updateItem.SubItems.Add(conf.items[i].category);
                updateItem.SubItems.Add(conf.items[i].shopCategory);
                updateItem.SubItems.Add(conf.items[i].owned.ToString());
                updateItem.SubItems.Add(conf.items[i].canBeBought.ToString());
                updateItem.SubItems.Add(conf.items[i].buyPrice.ToString());
                updateItem.SubItems.Add(conf.items[i].sellPrice.ToString());
                updateItem.SubItems.Add(conf.items[i].canOwnMultiple.ToString());
                updateItem.SubItems.Add(conf.items[i].image);
                updateItem.SubItems.Add(conf.items[i].skill1);
                updateItem.SubItems.Add(conf.items[i].skill2);
                updateItem.SubItems.Add(conf.items[i].skill3);
                updateItem.SubItems.Add(conf.items[i].passage);
                listView1.Items.Add(updateItem);
            }
        }

        // Update item in inventory
        private void button16_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                // Update item in shops
                int updatedItemInShop = 0;
                for(int i=0; i<conf.shops.Count; i++)
                {
                    for(int j=0; j<conf.shops[i].items.Count; j++)
                    {
                        if ((conf.shops[i].items[j].id == int.Parse(listView1.SelectedItems[0].Text)) &&
                            (conf.shops[i].items[j].type.Equals("ITEM")))
                        {
                            conf.shops[i].items[j].id = Convert.ToInt32(numericUpDown3.Value);
                            updatedItemInShop++;
                        }
                    }
                }

                // Update item in item list
                for(int i=0; i<conf.items.Count; i++)
                {
                    if (conf.items[i].ID == int.Parse(listView1.SelectedItems[0].Text))
                    {
                        conf.items[i].ID = Convert.ToInt32(numericUpDown3.Value);
                        conf.items[i].name = textBox7.Text;
                        conf.items[i].description = textBox2.Text;
                        conf.items[i].category = textBox8.Text;
                        conf.items[i].shopCategory = comboBox3.Text;
                        conf.items[i].owned = Convert.ToInt32(numericUpDown4.Value);
                        conf.items[i].canBeBought = checkBox13.Checked;
                        conf.items[i].buyPrice = Convert.ToInt32(numericUpDown5.Value);
                        conf.items[i].sellPrice = Convert.ToInt32(numericUpDown22.Value);
                        conf.items[i].canOwnMultiple = checkBox14.Checked;
                        conf.items[i].image = textBox10.Text;
                        conf.items[i].skill1 = textBox30.Text;
                        conf.items[i].skill2 = textBox29.Text;
                        conf.items[i].skill3 = textBox28.Text;
                        conf.items[i].passage = textBox5.Text;

                        // Copy file
                        if (!Directory.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.INVENTORY_PATH_PREFIX)))
                        {
                            Directory.CreateDirectory(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.INVENTORY_PATH_PREFIX));
                        }
                        if (File.Exists(textBox10.Text))
                        {
                            File.Copy(textBox10.Text, Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.INVENTORY_PATH_PREFIX, Path.GetFileName(textBox10.Text)));
                        }
                        // Delete old file
                        if (File.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.INVENTORY_PATH_PREFIX, conf.items[i].image)) && !conf.items[i].image.Equals(textBox10.Text))
                        {
                            DialogResult deleteOld = MessageBox.Show("The item image " + conf.items[i].image + " has changed. Delete the old one?", "Delete", MessageBoxButtons.YesNo);
                            if (deleteOld == DialogResult.Yes)
                            {
                                File.Delete(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.INVENTORY_PATH_PREFIX, conf.items[i].image));
                            }
                        }
                        conf.items[i].image = Path.GetFileName(textBox10.Text);
                        textBox10.Text = Path.GetFileName(textBox10.Text);

                        // update inventory
                        updateInventory();

                        MessageBox.Show("Item '" + conf.items[i].name + "' updated in item list and in " + updatedItemInShop +
                            " shop(s).");
                        return;
                    }
                }
            }
            MessageBox.Show("Item could not be updated.");
        }

        // Delete item from inventory
        private void button2_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem eachItem in listView1.SelectedItems)
            {
                var confirmResult = MessageBox.Show("Are you sure you want to delete the item with ID " + eachItem.Text + "?", "Confirm Delete", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    // Delete items from shop if required
                    int deletedItemsFromShops = 0;
                    for (int i = 0; i < conf.shops.Count; i++)
                    {
                        int j = 0;
                        while (j < conf.shops[i].items.Count)
                        {
                            if (conf.shops[i].items[j].id == int.Parse(eachItem.Text))
                            {
                                conf.shops[i].items.RemoveAt(j);
                                j = 0;
                                deletedItemsFromShops++;
                            }
                            else
                            {
                                j++;
                            }
                        }
                    }

                    for (int i = conf.items.Count - 1; i >= 0; i--)
                    {
                        if (conf.items[i].ID == int.Parse(eachItem.Text))
                        {
                            if (File.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.INVENTORY_PATH_PREFIX, conf.items[i].image)))
                            {
                                if (MessageBox.Show("Do you want to delete the item image, too?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    File.Delete(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.INVENTORY_PATH_PREFIX, conf.items[i].image));
                                }
                            }
                            
                            MessageBox.Show("Item '" + conf.items[i].name + "' was succesfully deleted from item list and deleted from " +
                                deletedItemsFromShops + " shop(s).");
                            conf.items.RemoveAt(i);
                        }
                    }
                    updateInventory();
                }
            }
        }

        // Load selected item into item group box
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                numericUpDown3.Value = int.Parse(listView1.SelectedItems[0].SubItems[0].Text);
                textBox7.Text = listView1.SelectedItems[0].SubItems[1].Text;
                textBox2.Text = listView1.SelectedItems[0].SubItems[2].Text;
                textBox8.Text = listView1.SelectedItems[0].SubItems[3].Text;
                comboBox3.Text = listView1.SelectedItems[0].SubItems[4].Text;
                numericUpDown4.Value = int.Parse(listView1.SelectedItems[0].SubItems[5].Text);
                checkBox13.Checked = bool.Parse(listView1.SelectedItems[0].SubItems[6].Text);
                numericUpDown5.Value = int.Parse(listView1.SelectedItems[0].SubItems[7].Text);
                numericUpDown22.Value = int.Parse(listView1.SelectedItems[0].SubItems[8].Text);
                checkBox14.Checked = bool.Parse(listView1.SelectedItems[0].SubItems[9].Text);
                textBox10.Text = listView1.SelectedItems[0].SubItems[10].Text;
                textBox30.Text = listView1.SelectedItems[0].SubItems[11].Text;
                textBox29.Text = listView1.SelectedItems[0].SubItems[12].Text;
                textBox28.Text = listView1.SelectedItems[0].SubItems[13].Text;
                textBox5.Text = listView1.SelectedItems[0].SubItems[14].Text;


                if (File.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.INVENTORY_PATH_PREFIX, textBox10.Text)))
                {
                    pictureBox2.Load(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.INVENTORY_PATH_PREFIX, textBox10.Text));
                }
            }
        }

        /*
         * Wardrobe
         */

        // Add item to wardrobe
        private void button18_Click(object sender, EventArgs e)
        {
            bool found = false;
            for (int i = 0; i < conf.clothing.Count; i++)
            {
                if (conf.clothing[i].ID == numericUpDown8.Value)
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                MessageBox.Show("The ID " + numericUpDown8.Value + " is already set.");
            }
            else
            {
                Clothing clothing = new Clothing();
                clothing.ID = Convert.ToInt32(numericUpDown8.Value);
                clothing.name = textBox14.Text;
                clothing.canBeBought = checkBox16.Checked;
                clothing.bodyPart = comboBox5.Text;
                clothing.description = textBox3.Text;
                clothing.skill1 = textBox20.Text;
                clothing.skill2 = textBox23.Text;
                clothing.skill3 = textBox24.Text;
                clothing.shopCategory = comboBox2.Text;
                clothing.category = textBox13.Text;
                clothing.image = textBox11.Text;
                clothing.buyPrice = Convert.ToInt32(numericUpDown6.Value);
                clothing.sellPrice = Convert.ToInt32(numericUpDown23.Value);
                clothing.canOwnMultiple = checkBox15.Checked;
                clothing.owned = Convert.ToInt32(numericUpDown7.Value);
                clothing.isWornAtBeginning = checkBox35.Checked;

                // Copy file
                if (!Directory.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX)))
                {
                    Directory.CreateDirectory(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX));
                }
                if (File.Exists(textBox11.Text))
                {
                    File.Copy(textBox11.Text, Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, Path.GetFileName(textBox11.Text)));
                }
                clothing.image = Path.GetFileName(textBox11.Text);
                textBox11.Text = Path.GetFileName(textBox11.Text);

                conf.clothing.Add(clothing);

                // Update wardrobe                
                updateClothing();

                numericUpDown8.Value += 1;
            }
        }


        // Update all cloth in the wardrobe
        private void updateClothing()
        {
            listView2.Items.Clear();
            for (int i = 0; i < conf.clothing.Count; i++)
            {
                ListViewItem updateItem = new ListViewItem();
                updateItem.Text = conf.clothing[i].ID.ToString();
                updateItem.SubItems.Add(conf.clothing[i].name);
                updateItem.SubItems.Add(conf.clothing[i].description);
                updateItem.SubItems.Add(conf.clothing[i].category);
                updateItem.SubItems.Add(conf.clothing[i].shopCategory);
                updateItem.SubItems.Add(conf.clothing[i].bodyPart);
                updateItem.SubItems.Add(conf.clothing[i].owned.ToString());
                updateItem.SubItems.Add(conf.clothing[i].isWornAtBeginning.ToString());
                updateItem.SubItems.Add(conf.clothing[i].canBeBought.ToString());
                updateItem.SubItems.Add(conf.clothing[i].buyPrice.ToString());
                updateItem.SubItems.Add(conf.clothing[i].sellPrice.ToString());
                updateItem.SubItems.Add(conf.clothing[i].canOwnMultiple.ToString());
                updateItem.SubItems.Add(conf.clothing[i].image);
                updateItem.SubItems.Add(conf.clothing[i].skill1);
                updateItem.SubItems.Add(conf.clothing[i].skill2);
                updateItem.SubItems.Add(conf.clothing[i].skill3);
                listView2.Items.Add(updateItem);
            }
        }

        // Update clothing in wardrobe
        private void button8_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count == 1)
            {
                for (int i = 0; i < conf.clothing.Count; i++)
                {
                    if (conf.clothing[i].ID == int.Parse(listView2.SelectedItems[0].Text))
                    {
                        conf.clothing[i].ID = Convert.ToInt32(numericUpDown8.Value);
                        conf.clothing[i].name = textBox14.Text;
                        conf.clothing[i].description = textBox3.Text;
                        conf.clothing[i].category = textBox13.Text;
                        conf.clothing[i].shopCategory = comboBox2.Text;
                        conf.clothing[i].bodyPart = comboBox5.Text;
                        conf.clothing[i].owned = Convert.ToInt32(numericUpDown7.Value);
                        conf.clothing[i].canBeBought = checkBox16.Checked;
                        conf.clothing[i].buyPrice = Convert.ToInt32(numericUpDown6.Value);
                        conf.clothing[i].sellPrice = Convert.ToInt32(numericUpDown23.Value);
                        conf.clothing[i].canOwnMultiple = checkBox15.Checked;
                        conf.clothing[i].image = textBox11.Text;
                        conf.clothing[i].skill1 = textBox20.Text;
                        conf.clothing[i].skill2 = textBox23.Text;
                        conf.clothing[i].skill3 = textBox24.Text;
                        conf.clothing[i].isWornAtBeginning = checkBox35.Checked;

                        // Copy file
                        if (!Directory.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX)))
                        {
                            Directory.CreateDirectory(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX));
                        }
                        if (File.Exists(textBox11.Text))
                        {
                            File.Copy(textBox11.Text, Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, Path.GetFileName(textBox11.Text)));
                        }
                        // Delete old file
                        if (File.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, conf.clothing[i].image)) && !conf.clothing[i].image.Equals(textBox11.Text))
                        {
                            DialogResult deleteOld = MessageBox.Show("The clothing image " + conf.clothing[i].image + " has changed. Delete the old one?", "Delete", MessageBoxButtons.YesNo);
                            if (deleteOld == DialogResult.Yes)
                            {
                                File.Delete(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, conf.clothing[i].image));
                            }
                        }
                        conf.clothing[i].image = Path.GetFileName(textBox11.Text);
                        textBox11.Text = Path.GetFileName(textBox11.Text);

                        updateClothing();

                        MessageBox.Show("Clothing updated.");
                        return;
                    }
                }
            }
            MessageBox.Show("Item could not be updated.");
        }

        // Delete clothing from wardrobe
        private void button19_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem eachItem in listView2.SelectedItems)
            {
                for (int i = conf.clothing.Count - 1; i >= 0; i--)
                {
                    if (conf.clothing[i].ID == int.Parse(eachItem.Text))
                    {
                        var confirmResult = MessageBox.Show("Are you sure you want to delete clothing with ID " + eachItem.Text + "?", "Confirm Delete", MessageBoxButtons.YesNo);
                        if (confirmResult == DialogResult.Yes)
                        {
                            if (File.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, conf.clothing[i].image)))
                            {
                                if (MessageBox.Show("Do you want to delete the clothing image, too?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    File.Delete(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, conf.clothing[i].image));
                                }
                            }
                            conf.clothing.RemoveAt(i);
                        }
                    }
                }
            }
            updateClothing();
        }

        private void listView2_DoubleClick(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count == 1)
            {
                numericUpDown8.Value = int.Parse(listView2.SelectedItems[0].SubItems[0].Text);
                textBox14.Text = listView2.SelectedItems[0].SubItems[1].Text;
                textBox3.Text = listView2.SelectedItems[0].SubItems[2].Text;
                textBox13.Text = listView2.SelectedItems[0].SubItems[3].Text;
                comboBox2.Text = listView2.SelectedItems[0].SubItems[4].Text;
                comboBox5.Text = listView2.SelectedItems[0].SubItems[5].Text;
                numericUpDown7.Value = int.Parse(listView2.SelectedItems[0].SubItems[6].Text);
                checkBox35.Checked = bool.Parse(listView2.SelectedItems[0].SubItems[7].Text);
                checkBox16.Checked = bool.Parse(listView2.SelectedItems[0].SubItems[8].Text);
                numericUpDown6.Value = int.Parse(listView2.SelectedItems[0].SubItems[9].Text);
                numericUpDown23.Value = int.Parse(listView2.SelectedItems[0].SubItems[10].Text);
                checkBox15.Checked = bool.Parse(listView2.SelectedItems[0].SubItems[11].Text);
                textBox11.Text = listView2.SelectedItems[0].SubItems[12].Text;
                textBox20.Text = listView2.SelectedItems[0].SubItems[13].Text;
                textBox23.Text = listView2.SelectedItems[0].SubItems[14].Text;
                textBox24.Text = listView2.SelectedItems[0].SubItems[15].Text;

                if (File.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, textBox11.Text)))
                {
                    pictureBox2.Load(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, textBox11.Text));
                }
            }
        }

        /*
         * Stats
         */

        // Add stats
        private void button20_Click(object sender, EventArgs e)
        {
            bool found = false;
            for (int i = 0; i < conf.stats.Count; i++)
            {
                if (conf.stats[i].ID == numericUpDown12.Value)
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                MessageBox.Show("The ID " + numericUpDown12.Value + " is already set.");
            }
            else
            {
                Stats stat = new Stats();
                stat.ID = Convert.ToInt32(numericUpDown12.Value);
                stat.name = textBox18.Text;
                stat.value = textBox16.Text;
                stat.unit = textBox19.Text;
                stat.image = textBox9.Text;
                stat.visible = checkBox24.Checked;
                stat.isSkill = comboBox8.Text;

                // Copy file
                if (!Directory.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.STATS_PATH_PREFIX)))
                {
                    Directory.CreateDirectory(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.STATS_PATH_PREFIX));
                }
                if (File.Exists(textBox9.Text))
                {
                    File.Copy(textBox9.Text, Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.STATS_PATH_PREFIX, Path.GetFileName(textBox9.Text)));
                }
                stat.image = Path.GetFileName(textBox9.Text);
                textBox9.Text = Path.GetFileName(textBox9.Text);

                conf.stats.Add(stat);

                // Update stats                
                updateStats();

                numericUpDown12.Value += 1;
            }
        }

        // Update stats
        private void updateStats()
        {
            listView5.Items.Clear();
            for (int i = 0; i < conf.stats.Count; i++)
            {
                ListViewItem updateItem = new ListViewItem();
                updateItem.Text = conf.stats[i].ID.ToString();
                updateItem.SubItems.Add(conf.stats[i].name);
                updateItem.SubItems.Add(conf.stats[i].value);
                updateItem.SubItems.Add(conf.stats[i].unit);
                updateItem.SubItems.Add(conf.stats[i].image);
                updateItem.SubItems.Add(conf.stats[i].description);
                updateItem.SubItems.Add(conf.stats[i].visible.ToString());
                updateItem.SubItems.Add(conf.stats[i].isSkill);
                listView5.Items.Add(updateItem);
            }
        }

        // Update stats from stats group box
        private void button10_Click(object sender, EventArgs e)
        {
            if (listView5.SelectedItems.Count == 1)
            {
                for (int i = 0; i < conf.stats.Count; i++)
                {
                    if (conf.stats[i].ID == int.Parse(listView5.SelectedItems[0].Text))
                    {
                        conf.stats[i].ID = Convert.ToInt32(numericUpDown12.Value);
                        conf.stats[i].name = textBox18.Text;
                        conf.stats[i].description = textBox4.Text;
                        conf.stats[i].value = textBox16.Text;
                        conf.stats[i].unit = textBox19.Text;
                        conf.stats[i].image = textBox9.Text;
                        conf.stats[i].visible = checkBox24.Checked;
                        conf.stats[i].isSkill = comboBox8.Text;

                        // Copy file
                        if (!Directory.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.STATS_PATH_PREFIX)))
                        {
                            Directory.CreateDirectory(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.STATS_PATH_PREFIX));
                        }
                        if (File.Exists(textBox9.Text))
                        {
                            File.Copy(textBox9.Text, Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.STATS_PATH_PREFIX, Path.GetFileName(textBox9.Text)));
                        }
                        // Delete old file
                        if (File.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.STATS_PATH_PREFIX, conf.stats[i].image)) && !conf.stats[i].image.Equals(textBox9.Text))
                        {
                            DialogResult deleteOld = MessageBox.Show("The stats image " + conf.stats[i].image + " has changed. Delete the old one?", "Delete", MessageBoxButtons.YesNo);
                            if (deleteOld == DialogResult.Yes)
                            {
                                File.Delete(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.STATS_PATH_PREFIX, conf.stats[i].image));
                            }
                        }
                        conf.stats[i].image = Path.GetFileName(textBox9.Text);
                        textBox9.Text = Path.GetFileName(textBox9.Text);

                        updateStats();

                        MessageBox.Show("Stats updated.");
                        return;
                    }
                }
            }
            MessageBox.Show("Stats could not be updated.");
        }

        // Delete stats
        private void button21_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem eachItem in listView5.SelectedItems)
            {
                for (int i = conf.stats.Count - 1; i >= 0; i--)
                {
                    if (conf.stats[i].ID == int.Parse(eachItem.Text))
                    {
                        var confirmResult = MessageBox.Show("Are you sure you want to delete stats with ID " + eachItem.Text + "?", "Confirm Delete", MessageBoxButtons.YesNo);
                        if (confirmResult == DialogResult.Yes)
                        {
                            if (File.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.STATS_PATH_PREFIX, conf.stats[i].image)))
                            {
                                if (MessageBox.Show("Do you want to delete the stats image, too?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    File.Delete(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.STATS_PATH_PREFIX, conf.stats[i].image));
                                }
                            }
                            conf.stats.RemoveAt(i);
                        }
                    }
                }
            }
            updateStats();
        }

        // Select stats
        private void listView5_DoubleClick(object sender, EventArgs e)
        {
            if (listView5.SelectedItems.Count == 1)
            {
                numericUpDown12.Value = int.Parse(listView5.SelectedItems[0].SubItems[0].Text);
                textBox18.Text = listView5.SelectedItems[0].SubItems[1].Text;
                textBox16.Text = listView5.SelectedItems[0].SubItems[2].Text;
                textBox19.Text = listView5.SelectedItems[0].SubItems[3].Text;
                textBox9.Text = listView5.SelectedItems[0].SubItems[4].Text;
                textBox4.Text = listView5.SelectedItems[0].SubItems[5].Text;
                checkBox24.Checked = listView5.SelectedItems[0].SubItems[6].Text.Equals("True");
                comboBox8.Text = listView5.SelectedItems[0].SubItems[7].Text;


                if (File.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.STATS_PATH_PREFIX, textBox9.Text))) {
                    pictureBox2.Load(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.STATS_PATH_PREFIX, textBox9.Text));
                }
            }
        }

        /*
         * Shops
         */
        
        // Add shop
        private void button11_Click(object sender, EventArgs e)
        {
            bool found = false;
            for (int i = 0; i < conf.shops.Count; i++)
            {
                if (conf.shops[i].ID == numericUpDown10.Value)
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                MessageBox.Show("The ID " + numericUpDown10.Value + " is already set.");
            }
            else
            {
                Shop shop = new Shop();
                shop.ID = Convert.ToInt32(numericUpDown10.Value);
                shop.name = textBox17.Text;
                shop.opening = dateTimePicker1.Value;
                shop.closing = dateTimePicker2.Value;

                conf.shops.Add(shop);

                // Update stats                
                updateShops();

                numericUpDown10.Value += 1;
            }
        }


        // Update all shops
        private void updateShops()
        {
            listView3.Items.Clear();
            for (int i = 0; i < conf.shops.Count; i++)
            {
                ListViewItem updateItem = new ListViewItem();
                updateItem.Text = conf.shops[i].ID.ToString();
                updateItem.SubItems.Add(conf.shops[i].name);
                updateItem.SubItems.Add(conf.shops[i].opening.ToString("HH:mm:ss"));
                updateItem.SubItems.Add(conf.shops[i].closing.ToString("HH:mm:ss"));
                listView3.Items.Add(updateItem);
            }
        }

        // Update shop in listview
        private void button9_Click(object sender, EventArgs e)
        {
            if (listView3.SelectedItems.Count == 1)
            {
                for (int i = 0; i < conf.shops.Count; i++)
                {
                    if (conf.shops[i].ID == int.Parse(listView3.SelectedItems[0].Text))
                    {
                        conf.shops[i].ID = Convert.ToInt32(numericUpDown10.Value);
                        conf.shops[i].name = textBox17.Text;
                        conf.shops[i].opening = dateTimePicker1.Value;
                        conf.shops[i].closing = dateTimePicker2.Value;

                        updateShops();

                        MessageBox.Show("Shops updated.");
                        return;
                    }
                }
            }
            MessageBox.Show("Shops could not be updated.");
        }

        // Delete shop
        private void button12_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem eachItem in listView3.SelectedItems)
            {
                for (int i = conf.shops.Count - 1; i >= 0; i--)
                {
                    if (conf.shops[i].ID == int.Parse(eachItem.Text))
                    {
                        var confirmResult = MessageBox.Show("Are you sure you want to delete the shop with ID " + eachItem.Text + "?", "Confirm Delete", MessageBoxButtons.YesNo);
                        if (confirmResult == DialogResult.Yes)
                        {
                            conf.shops.RemoveAt(i);
                        }
                    }
                }
            }
            updateShops();
        }

        // select a shop and its items
        private void listView3_DoubleClick(object sender, EventArgs e)
        {
            if (listView3.SelectedItems.Count == 1)
            {
                numericUpDown10.Value = int.Parse(listView3.SelectedItems[0].SubItems[0].Text);
                textBox17.Text = listView3.SelectedItems[0].SubItems[1].Text;
                dateTimePicker1.Value = DateTime.ParseExact(listView3.SelectedItems[0].SubItems[2].Text, "HH:mm:ss", null);
                dateTimePicker2.Value = DateTime.ParseExact(listView3.SelectedItems[0].SubItems[3].Text, "HH:mm:ss", null);

                // Get the right shop
                for (int i=0; i<conf.shops.Count; i++)
                {
                    if (conf.shops[i].ID == int.Parse(listView3.SelectedItems[0].Text))
                    {
                        label31.Text = "Items of shop \"" + conf.shops[i].name + "\":";
                        listView6.Items.Clear();
                        for(int j=0; j<conf.shops[i].items.Count; j++)
                        {
                            ListViewItem item = new ListViewItem();
                            item.Text = conf.shops[i].items[j].type;
                            item.SubItems.Add(conf.shops[i].items[j].id.ToString());
                            item.SubItems.Add(conf.shops[i].items[j].quantityStart.ToString());
                            item.SubItems.Add(conf.shops[i].items[j].quantityMax.ToString());
                            item.SubItems.Add(conf.shops[i].items[j].refillDelay.ToString());
                            listView6.Items.Add(item);
                        }
                    }
                }
            }
        }

        /*
         * Shop items
         */

        // Update an item
        private void button13_Click(object sender, EventArgs e)
        {
            if ((listView3.SelectedItems.Count == 1) && (listView6.SelectedItems.Count == 1))
            {
                // Get the right shop
                bool brk = false;
                for (int i = 0; i < conf.shops.Count; i++)
                {
                    if (conf.shops[i].ID == int.Parse(listView3.SelectedItems[0].Text))
                    {
                        for (int j = 0; j < conf.shops[i].items.Count; j++)
                        {
                            if (conf.shops[i].items[j].id == int.Parse(listView6.SelectedItems[0].SubItems[1].Text ))
                            {
                                conf.shops[i].items[j].type = comboBox6.Text;
                                conf.shops[i].items[j].id = int.Parse(comboBox1.Text);
                                conf.shops[i].items[j].quantityStart = Convert.ToInt32(numericUpDown11.Value);
                                conf.shops[i].items[j].quantityMax = Convert.ToInt32(numericUpDown20.Value);
                                conf.shops[i].items[j].refillDelay = Convert.ToInt32(numericUpDown21.Value);
                                listView6.SelectedItems[0].SubItems[0].Text = comboBox6.Text;
                                listView6.SelectedItems[0].SubItems[1].Text = comboBox1.Text;
                                listView6.SelectedItems[0].SubItems[2].Text = numericUpDown11.Value.ToString();
                                listView6.SelectedItems[0].SubItems[3].Text = Convert.ToInt32(numericUpDown20.Value).ToString();
                                listView6.SelectedItems[0].SubItems[4].Text = Convert.ToInt32(numericUpDown21.Value).ToString();
                                brk = true;
                                break;
                            }
                        }
                        if (brk) break;
                    }
                }
            } else
            {
                MessageBox.Show("Please select one shop to update its items.");
            }
        }

        // Add new item to shop
        private void button22_Click(object sender, EventArgs e)
        {
            if (listView3.SelectedItems.Count == 1)
            {
                // Get active shop by id
                int activeShopId = -1;
                for(int i=0; i<conf.shops.Count; i++)
                {
                    if (conf.shops[i].ID == int.Parse(listView3.SelectedItems[0].Text)) {
                        activeShopId = i;
                        break;
                    }
                }

                if (activeShopId == -1)
                {
                    MessageBox.Show("Could not find active shop.");
                    return;
                }

                // Has already an item of that kind?
                bool itemAlreadyInList = false;
                for(int i=0; i<conf.shops[activeShopId].items.Count; i++)
                {
                    if ((conf.shops[activeShopId].items[i].id == int.Parse(comboBox1.Text)) && (conf.shops[activeShopId].items[i].type == comboBox6.Text))
                    {
                        itemAlreadyInList = true;
                        break;
                    }
                }

                if (itemAlreadyInList)
                {
                    MessageBox.Show("Item is already in list. Please update.");
                } else
                {
                    conf.shops[activeShopId].items.Add(new ShopItem(comboBox6.Text, int.Parse(comboBox1.Text), Convert.ToInt32(numericUpDown11.Value),
                        Convert.ToInt32(numericUpDown20.Value), Convert.ToInt32(numericUpDown21.Value)));
                    ListViewItem item = new ListViewItem();
                    item.Text = comboBox6.Text;
                    item.SubItems.Add(comboBox1.Text);
                    item.SubItems.Add(Convert.ToInt32(numericUpDown11.Value).ToString());
                    item.SubItems.Add(Convert.ToInt32(numericUpDown20.Value).ToString());
                    item.SubItems.Add(Convert.ToInt32(numericUpDown21.Value).ToString());
                    listView6.Items.Add(item);
                }
            } else
            {
                MessageBox.Show("Please select a shop first.");
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            if ((listView3.SelectedItems.Count == 1) && (listView6.SelectedItems.Count == 1))
            {
                // Get active shop by id
                int activeShopId = -1;
                for (int i = 0; i < conf.shops.Count; i++)
                {
                    if (conf.shops[i].ID == int.Parse(listView3.SelectedItems[0].Text))
                    {
                        activeShopId = i;
                        break;
                    }
                }

                if (activeShopId == -1)
                {
                    MessageBox.Show("Could not find active shop.");
                    return;
                }

                for (int i = 0; i < conf.shops[activeShopId].items.Count; i++)
                {
                    if (conf.shops[activeShopId].items[i].id == int.Parse(listView6.SelectedItems[0].Text))
                    {
                        conf.shops[activeShopId].items.RemoveAt(i);
                        listView6.Items.Remove(listView6.SelectedItems[0]);
                        break;
                    }
                }
            }
        }

        private void tabPage5_Enter(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            if (comboBox6.Text.Equals("ITEM"))
            {
                for (int i = 0; i < conf.items.Count; i++)
                {
                    comboBox1.Items.Add(conf.items[i].ID);
                }
            } else
            {
                for (int i = 0; i < conf.clothing.Count; i++)
                {
                    comboBox1.Items.Add(conf.clothing[i].ID);
                }
            }
            listView6.Items.Clear();
            label31.Text = "Items of shop -:";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox6.Text.Equals("ITEM"))
            {
                for (int i = 0; i < conf.items.Count; i++)
                {
                    if (comboBox1.Text.Equals(conf.items[i].ID.ToString()))
                    {
                        label73.Text = "(" + conf.items[i].name + ")";

                        if (File.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.INVENTORY_PATH_PREFIX, conf.items[i].image))) {
                            pictureBox2.Load(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.INVENTORY_PATH_PREFIX, conf.items[i].image));
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < conf.clothing.Count; i++)
                {
                    if (comboBox1.Text.Equals(conf.clothing[i].ID.ToString()))
                    {
                        label73.Text = "(" + conf.clothing[i].name + ")";

                        if (File.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, conf.clothing[i].image)))
                        {
                            pictureBox2.Load(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, conf.clothing[i].image));
                        }
                    }
                }
            }
        }

        private void listView6_DoubleClick(object sender, EventArgs e)
        {
            if (listView6.SelectedItems.Count == 1)
            {
                comboBox6.SelectedIndex = comboBox6.FindStringExact(listView6.SelectedItems[0].SubItems[0].Text);
                comboBox1.Text = listView6.SelectedItems[0].SubItems[1].Text;
                numericUpDown11.Value = int.Parse(listView6.SelectedItems[0].SubItems[2].Text);
                numericUpDown20.Value = int.Parse(listView6.SelectedItems[0].SubItems[3].Text);
                numericUpDown21.Value = int.Parse(listView6.SelectedItems[0].SubItems[4].Text);
            }
        }

        /*
         * Jobs
         */

        // Add new job
        private void button24_Click(object sender, EventArgs e)
        {
            bool found = false;
            for (int i = 0; i < conf.jobs.Count; i++)
            {
                if (conf.jobs[i].ID == numericUpDown15.Value)
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                MessageBox.Show("The ID " + numericUpDown15.Value + " is already set.");
            }
            else
            {
                Job job = new Job();
                job.ID = Convert.ToInt32(numericUpDown15.Value);
                job.name = textBox22.Text;
                job.available = checkBox18.Checked;
                job.cooldown = Convert.ToInt32(numericUpDown14.Value);
                job.category = textBox21.Text;
                job.image = textBox15.Text;
                job.description = textBox1.Text;
                job.duration = Convert.ToInt32(numericUpDown19.Value);
                job.rewardMoney = Convert.ToInt32(numericUpDown9.Value);

                // Copy file
                if (!Directory.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.JOBS_PATH_PREFIX)))
                {
                    Directory.CreateDirectory(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.JOBS_PATH_PREFIX));
                }
                if (File.Exists(textBox15.Text))
                {
                    File.Copy(textBox15.Text, Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.JOBS_PATH_PREFIX, Path.GetFileName(textBox15.Text)));
                }
                job.image = Path.GetFileName(textBox15.Text);
                textBox15.Text = Path.GetFileName(textBox15.Text);

                conf.jobs.Add(job);

                // Update jobs                
                updateJobs();

                numericUpDown15.Value += 1;
            }
        }

        // Update Jobs
        private void updateJobs()
        {
            listView4.Items.Clear();
            for (int i = 0; i < conf.jobs.Count; i++)
            {
                ListViewItem updateItem = new ListViewItem();
                updateItem.Text = conf.jobs[i].ID.ToString();
                updateItem.SubItems.Add(conf.jobs[i].name);
                updateItem.SubItems.Add(conf.jobs[i].description);
                updateItem.SubItems.Add(conf.jobs[i].category);
                updateItem.SubItems.Add(conf.jobs[i].available.ToString());
                updateItem.SubItems.Add(conf.jobs[i].rewardMoney.ToString());
                updateItem.SubItems.Add(conf.jobs[i].cooldown.ToString());
                updateItem.SubItems.Add(conf.jobs[i].duration.ToString());
                updateItem.SubItems.Add(conf.jobs[i].image);
                updateItem.SubItems.Add(conf.jobs[i].passage);
                listView4.Items.Add(updateItem);
            }
        }

        // Update current job
        private void button15_Click(object sender, EventArgs e)
        {
            if (listView4.SelectedItems.Count == 1)
            {
                for (int i = 0; i < conf.jobs.Count; i++)
                {
                    if (conf.jobs[i].ID == int.Parse(listView4.SelectedItems[0].Text))
                    {
                        conf.jobs[i].ID = Convert.ToInt32(numericUpDown15.Value);
                        conf.jobs[i].name = textBox22.Text;
                        conf.jobs[i].available = checkBox18.Checked;
                        conf.jobs[i].cooldown = Convert.ToInt32(numericUpDown14.Value);
                        conf.jobs[i].category = textBox21.Text;
                        conf.jobs[i].duration = Convert.ToInt32(numericUpDown19.Value);
                        conf.jobs[i].image = textBox15.Text;
                        conf.jobs[i].description = textBox1.Text;
                        conf.jobs[i].rewardMoney = Convert.ToInt32(numericUpDown9.Value);
                        conf.jobs[i].passage = textBox6.Text;

                        // Copy file
                        if (!Directory.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.JOBS_PATH_PREFIX)))
                        {
                            Directory.CreateDirectory(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.JOBS_PATH_PREFIX));
                        }
                        if (File.Exists(textBox10.Text))
                        {
                            File.Copy(textBox15.Text, Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.JOBS_PATH_PREFIX, Path.GetFileName(textBox15.Text)));
                        }
                        // Delete old file
                        if (File.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.JOBS_PATH_PREFIX, conf.jobs[i].image)) && !conf.jobs[i].image.Equals(textBox15.Text))
                        {
                            DialogResult deleteOld = MessageBox.Show("The jobs image " + conf.jobs[i].image + " has changed. Delete the old one?", "Delete", MessageBoxButtons.YesNo);
                            if (deleteOld == DialogResult.Yes)
                            {
                                File.Delete(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.JOBS_PATH_PREFIX, conf.jobs[i].image));
                            }
                        }
                        conf.jobs[i].image = Path.GetFileName(textBox15.Text);
                        textBox15.Text = Path.GetFileName(textBox15.Text);

                        updateJobs();

                        MessageBox.Show("Jobs updated.");
                        return;
                    }
                }
            }
            MessageBox.Show("Job could not be updated.");
        }

        // Delete job
        private void button25_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem eachItem in listView4.SelectedItems)
            {
                for (int i = conf.jobs.Count - 1; i >= 0; i--)
                {
                    if (conf.jobs[i].ID == int.Parse(eachItem.Text))
                    {
                        var confirmResult = MessageBox.Show("Are you sure you want to delete jobs with ID " + eachItem.Text + "?", "Confirm Delete", MessageBoxButtons.YesNo);
                        if (confirmResult == DialogResult.Yes)
                        {
                            if (File.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.JOBS_PATH_PREFIX, conf.jobs[i].image)))
                            {
                                if (MessageBox.Show("Do you want to delete the job image, too?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    File.Delete(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.JOBS_PATH_PREFIX, conf.jobs[i].image));
                                }
                            }
                            conf.jobs.RemoveAt(i);
                        }
                    }
                }
            }
            updateJobs();
        }

        // Select job
        private void listView4_DoubleClick(object sender, EventArgs e)
        {
            if (listView4.SelectedItems.Count == 1)
            {
                numericUpDown15.Value = int.Parse(listView4.SelectedItems[0].SubItems[0].Text);
                textBox22.Text = listView4.SelectedItems[0].SubItems[1].Text;
                textBox1.Text = listView4.SelectedItems[0].SubItems[2].Text;
                checkBox18.Checked = bool.Parse(listView4.SelectedItems[0].SubItems[4].Text);
                numericUpDown14.Value = int.Parse(listView4.SelectedItems[0].SubItems[6].Text);
                textBox21.Text = listView4.SelectedItems[0].SubItems[3].Text;
                textBox15.Text = listView4.SelectedItems[0].SubItems[8].Text;
                numericUpDown19.Value = int.Parse(listView4.SelectedItems[0].SubItems[7].Text);
                numericUpDown9.Value = int.Parse(listView4.SelectedItems[0].SubItems[5].Text);
                textBox6.Text = listView4.SelectedItems[0].SubItems[9].Text;

                if (File.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.JOBS_PATH_PREFIX, textBox15.Text)))
                {
                    pictureBox2.Load(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.JOBS_PATH_PREFIX, textBox15.Text));
                }

                // Get the right job
                for (int i = 0; i < conf.jobs.Count; i++)
                {
                    if (conf.jobs[i].ID == int.Parse(listView4.SelectedItems[0].SubItems[0].Text))
                    {
                        label76.Text = "Reward items for job \"" + listView4.SelectedItems[0].SubItems[1].Text + "\":";
                        listView8.Items.Clear();
                        for (int j = 0; j < conf.jobs[i].rewardItems.Count; j++)
                        {
                            ListViewItem item = new ListViewItem();
                            item.Text = conf.jobs[i].rewardItems[j].type;
                            item.SubItems.Add(conf.jobs[i].rewardItems[j].ID.ToString());
                            item.SubItems.Add(conf.jobs[i].rewardItems[j].amount.ToString());
                            listView8.Items.Add(item);
                        }
                    }
                }
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "|*.*||*.png||*.jpg||*.gif";
            openFileDialog1.Title = "Open Image File";
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                textBox10.Text = openFileDialog1.FileName;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "|*.*||*.png||*.jpg||*.gif";
            openFileDialog1.Title = "Open Image File";
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                textBox11.Text = Path.GetFileName(openFileDialog1.FileName);
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "|*.*||*.png||*.jpg||*.gif";
            openFileDialog1.Title = "Open Image File";
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                textBox15.Text = Path.GetFileName(openFileDialog1.FileName);
            }
        }

        private void button26_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "|*.*||*.png||*.jpg||*.gif";
            openFileDialog1.Title = "Open Image File";
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                textBox37.Text = Path.GetFileName(openFileDialog1.FileName);
            }
        }



        /*
         * Characters
         */

        // Add new character
        private void button28_Click(object sender, EventArgs e)
        {
            bool found = false;
            for (int i = 0; i < conf.characters.Count; i++)
            {
                if (conf.characters[i].ID == numericUpDown17.Value)
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                MessageBox.Show("The ID " + numericUpDown17.Value + " is already set.");
            }
            else
            {
                Character character = new Character();
                character.ID = Convert.ToInt32(numericUpDown17.Value);
                character.name = textBox40.Text;
                character.age = Convert.ToInt32(numericUpDown18.Value);
                character.skill1 = textBox36.Text;
                character.description = textBox38.Text;
                character.known = checkBox17.Checked;
                character.category = textBox39.Text;
                character.image = textBox37.Text;
                character.skill2 = textBox35.Text;
                character.gender = textBox45.Text;
                character.job = textBox46.Text;
                character.relation = Convert.ToInt32(numericUpDown16.Value);
                character.color = HexConverter(panel1.BackColor);
                character.skill3 = textBox34.Text;

                // Copy file
                if (!Directory.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CHARACTERS_PATH_PREFIX)))
                {
                    Directory.CreateDirectory(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CHARACTERS_PATH_PREFIX));
                }
                if (File.Exists(textBox37.Text))
                {
                    File.Copy(textBox37.Text, Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CHARACTERS_PATH_PREFIX, Path.GetFileName(textBox37.Text)));
                }
                character.image = Path.GetFileName(textBox37.Text);
                textBox37.Text = Path.GetFileName(textBox37.Text);

                conf.characters.Add(character);

                // Update jobs                
                updateCharacters();

                numericUpDown17.Value += 1;
            }
        }

        // Update characters
        private void updateCharacters()
        {
            listView7.Items.Clear();
            for (int i = 0; i < conf.characters.Count; i++)
            {
                ListViewItem updateItem = new ListViewItem();
                updateItem.Text = conf.characters[i].ID.ToString();
                updateItem.SubItems.Add(conf.characters[i].name);
                updateItem.SubItems.Add(conf.characters[i].category);
                updateItem.SubItems.Add(conf.characters[i].description);
                updateItem.SubItems.Add(conf.characters[i].known.ToString());
                updateItem.SubItems.Add(conf.characters[i].age.ToString());
                updateItem.SubItems.Add(conf.characters[i].gender);
                updateItem.SubItems.Add(conf.characters[i].job);
                updateItem.SubItems.Add(conf.characters[i].relation.ToString());
                updateItem.SubItems.Add(conf.characters[i].image);
                updateItem.SubItems.Add(conf.characters[i].color);
                updateItem.SubItems.Add(conf.characters[i].skill1);
                updateItem.SubItems.Add(conf.characters[i].skill2);
                updateItem.SubItems.Add(conf.characters[i].skill3);
                listView7.Items.Add(updateItem);
            }
        }
    
        // Update character
        private void button27_Click(object sender, EventArgs e)
        {
            if (listView7.SelectedItems.Count == 1)
            {
                for (int i = 0; i < conf.characters.Count; i++)
                {
                    if (conf.characters[i].ID == int.Parse(listView7.SelectedItems[0].Text))
                    {
                        conf.characters[i].ID = Convert.ToInt32(numericUpDown17.Value);
                        conf.characters[i].name = textBox40.Text;
                        conf.characters[i].age = Convert.ToInt32(numericUpDown18.Value);
                        conf.characters[i].skill1 = textBox36.Text;
                        conf.characters[i].description = textBox38.Text;
                        conf.characters[i].known = checkBox17.Checked;
                        conf.characters[i].image = textBox37.Text;
                        conf.characters[i].skill2 = textBox35.Text;
                        conf.characters[i].gender = textBox45.Text;
                        conf.characters[i].job = textBox46.Text;
                        conf.characters[i].relation = Convert.ToInt32(numericUpDown16.Value);
                        conf.characters[i].color = HexConverter(panel1.BackColor);
                        conf.characters[i].skill3 = textBox34.Text;

                        // Copy file
                        if (!Directory.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CHARACTERS_PATH_PREFIX)))
                        {
                            Directory.CreateDirectory(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CHARACTERS_PATH_PREFIX));
                        }
                        if (File.Exists(textBox37.Text))
                        {
                            File.Copy(textBox37.Text, Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CHARACTERS_PATH_PREFIX, Path.GetFileName(textBox37.Text)));
                        }
                        // Delete old file
                        if (File.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CHARACTERS_PATH_PREFIX, conf.characters[i].image)) && !conf.characters[i].image.Equals(textBox37.Text))
                        {
                            DialogResult deleteOld = MessageBox.Show("The character image " + conf.characters[i].image + " has changed. Delete the old one?", "Delete", MessageBoxButtons.YesNo);
                            if (deleteOld == DialogResult.Yes)
                            {
                                File.Delete(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CHARACTERS_PATH_PREFIX, conf.characters[i].image));
                            }
                        }
                        conf.characters[i].image = Path.GetFileName(textBox37.Text);
                        textBox37.Text = Path.GetFileName(textBox37.Text);

                        updateCharacters();

                        MessageBox.Show("Characters updated.");
                        return;
                    }
                }
            }
            MessageBox.Show("Characters could not be updated.");
        }

        private void button29_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem eachItem in listView7.SelectedItems)
            {
                for (int i = conf.characters.Count - 1; i >= 0; i--)
                {
                    if (conf.characters[i].ID == int.Parse(eachItem.Text))
                    {
                        var confirmResult = MessageBox.Show("Are you sure you want to delete character with ID " + eachItem.Text + "?", "Confirm Delete", MessageBoxButtons.YesNo);
                        if (confirmResult == DialogResult.Yes)
                        {
                            if ( File.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CHARACTERS_PATH_PREFIX, conf.characters[i].image)))
                            {
                                if (MessageBox.Show("Do you want to delete the character image, too?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    File.Delete(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CHARACTERS_PATH_PREFIX, conf.characters[i].image));
                                }
                            }
                            conf.characters.RemoveAt(i);
                        }
                    }
                }
            }
            updateCharacters();
        }

        private void listView7_DoubleClick(object sender, EventArgs e)
        {
            if (listView7.SelectedItems.Count == 1)
            {
                numericUpDown17.Value = int.Parse(listView7.SelectedItems[0].SubItems[0].Text);
                textBox40.Text = listView7.SelectedItems[0].SubItems[1].Text;
                textBox39.Text = listView7.SelectedItems[0].SubItems[2].Text;
                textBox38.Text = listView7.SelectedItems[0].SubItems[3].Text;
                checkBox17.Checked = bool.Parse(listView7.SelectedItems[0].SubItems[4].Text);
                numericUpDown18.Value = int.Parse(listView7.SelectedItems[0].SubItems[5].Text);
                textBox45.Text = listView7.SelectedItems[0].SubItems[6].Text;
                textBox46.Text = listView7.SelectedItems[0].SubItems[7].Text;
                numericUpDown16.Value = int.Parse(listView7.SelectedItems[0].SubItems[8].Text);
                textBox37.Text = listView7.SelectedItems[0].SubItems[9].Text;
                panel1.BackColor = ColorTranslator.FromHtml(listView7.SelectedItems[0].SubItems[10].Text);
                textBox36.Text = listView7.SelectedItems[0].SubItems[11].Text;
                textBox35.Text = listView7.SelectedItems[0].SubItems[12].Text;
                textBox34.Text = listView7.SelectedItems[0].SubItems[13].Text;

                if (File.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CHARACTERS_PATH_PREFIX, textBox37.Text)))
                {
                    pictureBox2.Load(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CHARACTERS_PATH_PREFIX, textBox37.Text));
                }
            }
        }

        public bool newConfigFromTemplate(string newProjectName, string templateDir, bool askToSave = true)
        {
            var confirmResult = DialogResult.Yes;

            if (askToSave)
            {
                confirmResult = MessageBox.Show("Do you want to save the current project before starting a new one?", "Save", MessageBoxButtons.YesNoCancel);
                if (confirmResult == DialogResult.Yes)
                {
                    save();
                }
            }

            if (confirmResult != DialogResult.Cancel)
            {

                // Start in application directory
                string projectsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "projects");
                if (!Directory.Exists(projectsDir))
                {
                    Directory.CreateDirectory(projectsDir);
                }

                string pd = Path.Combine(projectsDir, newProjectName);


                if ((Directory.Exists(pd) && (Directory.GetFiles(pd).Length > 0))
                    || (!Directory.GetParent(pd).FullName.Equals(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "projects"))))
                {
                    MessageBox.Show("'" + newProjectName + "' already exists.");
                }
                else
                {
                    if (!Directory.Exists(pd))
                    {
                        Directory.CreateDirectory(pd);
                    }

                    // Copy from template
                    Helper.Copy(templateDir, pd);

                    // Load template conf and update
                    string projectFile = "";

                    if (File.Exists(Path.Combine(pd, "project.tfcx")))
                    {
                        projectFile = Path.Combine(pd, "project.tfcx");
                    }

                    if (projectFile.Equals(""))
                    {
                        MessageBox.Show("This template does not contain a 'project.tfcx'.");
                        return false;
                    }

                    string ext = Path.GetExtension(projectFile);

                    if (ext.ToUpper().Equals(".TFCX")) // XML
                    {
                        TextReader reader = null;
                        try
                        {
                            var serializer = new XmlSerializer(typeof(Configuration));
                            reader = new StreamReader(projectFile);
                            conf = (Configuration)serializer.Deserialize(reader);

                            projectName = newProjectName;
                            projectDir = pd;
                            this.Text = Form1.APP_TITLE + " - " + projectName;
                            // Update comboboxes
                            /*comboBox3.Items.Clear();
                            for (int i = 0; i < conf.shops.Count; i++)
                            {
                                comboBox3.Items.Add(conf.shops[i].name);
                            }
                            comboBox2.Items.Clear();
                            for (int i = 0; i < conf.shops.Count; i++)
                            {
                                comboBox2.Items.Add(conf.shops[i].name);
                            }
                            comboBox4.Items.Clear();
                            for (int i = 0; i < conf.items.Count; i++)
                            {
                                comboBox4.Items.Add(conf.items[i].ID);
                            }*/
                            updateFromConf(conf);
                        }
                        finally
                        {
                            if (reader != null)
                                reader.Close();
                        }
                        return true;
                    }
                    conf = new Configuration(true);
                    updateFromConf(conf);
                    save();
                    return true;
                }

            }
            return false;
        }

        public bool newConfig(string newProjectName, bool askToSave = true)
        {
            var confirmResult = DialogResult.Yes;

            if (askToSave)
            {
                confirmResult = MessageBox.Show("Do you want to save the current project before starting a new one?", "Save", MessageBoxButtons.YesNoCancel);
                if (confirmResult == DialogResult.Yes)
                {
                    save();
                }
            }

            if (confirmResult != DialogResult.Cancel)
            {

                // Start in application directory
                string projectsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "projects");
                if (!Directory.Exists(projectsDir))
                {
                    Directory.CreateDirectory(projectsDir);
                }

                string pd = Path.Combine(projectsDir, newProjectName);


                if ((Directory.Exists(pd) && (Directory.GetFiles(pd).Length > 0))
                    || (!Directory.GetParent(pd).FullName.Equals(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "projects"))))
                {
                    MessageBox.Show("'" + newProjectName + "' already exists.");
                }
                else
                {
                    if (!Directory.Exists(pd))
                    {
                        Directory.CreateDirectory(pd);
                    }

                    projectName = newProjectName;
                    projectDir = pd;
                    this.Text = Form1.APP_TITLE + " - " + projectName;
                    conf = new Configuration(true);
                    updateFromConf(conf);
                    save();
                    return true;
                }

            }
            return false;
        }

        public bool loadConf(string projectName, bool askToSave = true)
        {
            var confirmResult = DialogResult.Yes;

            if (askToSave)
            {
                confirmResult = MessageBox.Show("Do you want to save the current project before loading?", "Save", MessageBoxButtons.YesNoCancel);
                if (confirmResult == DialogResult.Yes)
                {
                    save();
                }
            }

            if (confirmResult != DialogResult.Cancel)
            {
                string pd = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "projects", projectName);

                if (Directory.Exists(pd))
                {
                    string projectFile = "";

                    if (File.Exists(Path.Combine(pd, "project.tfcx")))
                    {
                        projectFile = Path.Combine(pd, "project.tfcx");
                    }

                    if (projectFile.Equals(""))
                    {
                        MessageBox.Show("This folder does not contain a 'project.tfcx'.");
                        return false;
                    }


                    string ext = Path.GetExtension(projectFile);

                    if (ext.ToUpper().Equals(".TFCX")) // XML
                    {
                        TextReader reader = null;
                        try
                        {
                            var serializer = new XmlSerializer(typeof(Configuration));
                            reader = new StreamReader(projectFile);
                            conf = (Configuration)serializer.Deserialize(reader);

                            projectName = new DirectoryInfo(pd).Name;
                            this.Text = Form1.APP_TITLE + " - " + projectName;
                            projectDir = pd;
                            // Update comboboxes
                            /* comboBox3.Items.Clear();
                             for (int i = 0; i < conf.shops.Count; i++)
                             {
                                 comboBox3.Items.Add(conf.shops[i].name);
                             }
                             comboBox2.Items.Clear();
                             for (int i = 0; i < conf.shops.Count; i++)
                             {
                                 comboBox2.Items.Add(conf.shops[i].name);
                             }
                             comboBox4.Items.Clear();
                             for (int i = 0; i < conf.items.Count; i++)
                             {
                                 comboBox4.Items.Add(conf.items[i].ID);
                             }*/
                            updateFromConf(conf);
                        }
                        finally
                        {
                            if (reader != null)
                                reader.Close();
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private void saveConfAs()
        {
 
            using (var project_folder_dialog = new FolderBrowserDialog())
            {
                // Start in application directory
                project_folder_dialog.SelectedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "projects");
                if (!Directory.Exists(project_folder_dialog.SelectedPath))
                {
                    Directory.CreateDirectory(project_folder_dialog.SelectedPath);
                }

                DialogResult result = project_folder_dialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(project_folder_dialog.SelectedPath))
                {
                    if ((Directory.Exists(project_folder_dialog.SelectedPath) && (Directory.GetFiles(project_folder_dialog.SelectedPath).Length > 0))
                        || (!Directory.GetParent(project_folder_dialog.SelectedPath).FullName.Equals(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "projects"))))
                    {
                        MessageBox.Show("Please create a new folder or select an empty folder in the 'projects' directory.");
                    }
                    else
                    {
                        if (!Directory.Exists(project_folder_dialog.SelectedPath))
                        {
                            Directory.CreateDirectory(project_folder_dialog.SelectedPath);
                        }

                        // Todo copy all folders from fileName to new Dir
                        string[] filesInCurrentDir = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "projects", projectName)); // "^(?!.*projects.tfcx)"
                        filesInCurrentDir = Array.FindAll(filesInCurrentDir, file => !file.ToUpper().StartsWith("PROJECT.TFCX"));
                        string[] dirsInCurrentDir = Directory.GetDirectories(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "projects", projectName));
                        int totalSubs = filesInCurrentDir.Length + dirsInCurrentDir.Length;
                        if (totalSubs > 0)
                        {
                            DialogResult copyFiles = MessageBox.Show("There are multiple files in the current project. Do you want to copy them to the new location?", "Copy all", MessageBoxButtons.YesNo);
                            if (copyFiles == DialogResult.Yes)
                            {
                                Helper.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "projects", projectName), project_folder_dialog.SelectedPath);
                                if (File.Exists(Path.Combine(project_folder_dialog.SelectedPath, "project.tfcx")))
                                {
                                    File.Delete(Path.Combine(project_folder_dialog.SelectedPath, "project.tfcx"));
                                }
                            }
                        }

                        projectName = new DirectoryInfo(project_folder_dialog.SelectedPath).Name;
                        this.Text = Form1.APP_TITLE + " - " + projectName;
                        projectDir = project_folder_dialog.SelectedPath;
                        save();
                    }

                }
            }


        }

        private void save()
        {
            if (!projectName.Equals(""))
            {
                // Find the project file
                string projectFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "projects", projectName, "project.tfcx");

                if (Path.GetExtension(projectFile).ToUpper().Equals(".TFCX")) // XML
                {

                    TextWriter writer = null;
                    try
                    {
                        var serializer = new XmlSerializer(typeof(Configuration));
                        writer = new StreamWriter(projectFile);
                        serializer.Serialize(writer, conf);
                    }
                    finally
                    {
                        if (writer != null)
                        {
                            writer.Close();
                        }
                    }
                }

            } else
            {
                saveConfAs();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Welcome welcomeForm = new Welcome(this);
            welcomeForm.ShowDialog();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Welcome welcomeForm = new Welcome(this);
            welcomeForm.ShowDialog();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveConfAs();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save();
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControl1.SelectedTab.Text == "Inventory")
            {
                comboBox3.Items.Clear();
                for(int i=0; i<conf.shops.Count; i++)
                {
                    comboBox3.Items.Add(conf.shops[i].name);
                }
            }
            else if (tabControl1.SelectedTab.Text == "Clothing")
            {
                comboBox2.Items.Clear();
                for (int i = 0; i < conf.shops.Count; i++)
                {
                    comboBox2.Items.Add(conf.shops[i].name);
                }
            } else if (tabControl1.SelectedTab.Text == "Jobs")
            {
                comboBox7.Text = "ITEM";
                comboBox4.Items.Clear();
                for(int i=0; i<conf.items.Count; i++)
                {
                    comboBox4.Items.Add(conf.items[i].ID);
                }
            } else if (tabControl1.SelectedTab.Text == "Shops")
            {
                comboBox6.Text = "ITEM";
                comboBox1.Items.Clear();
                for (int i = 0; i < conf.items.Count; i++)
                {
                    comboBox1.Items.Add(conf.items[i].ID);
                }
            } else if (tabControl1.SelectedTab.Text == "Configuration")
            {
                listBox1.Items.Clear();
                for(int i=0; i<conf.includedFiles.Count; i++)
                {
                    listBox1.Items.Add(conf.includedFiles[i]);
                }
            } else if (tabControl1.SelectedTab.Text == "Daytime")
            {
                switch(conf.daytimeFormat)
                {
                    case 0: radioButton1.Checked = true; break;
                    case 1: radioButton2.Checked = true; break;
                    case 2: radioButton3.Checked = true; break;
                    case 3: radioButton4.Checked = true; break;
                }
            }
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            conf.clothingLinkInSidebar = checkBox8.Checked;
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            conf.statsLinkInSidebar = checkBox7.Checked;
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            conf.daytimeInSidebar = checkBox10.Checked;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            conf.daytimeFormat = 0;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            conf.daytimeFormat = 1;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            conf.daytimeFormat = 2;
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            conf.moneyInSidebar = checkBox11.Checked;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            conf.startMoney = Convert.ToInt32(numericUpDown1.Value);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            conf.moneyPerDay = Convert.ToInt32(numericUpDown2.Value);
            if (conf.moneyPerDay != 0)
            {
                if (!conf.daytimeActive)
                {
                    MessageBox.Show("To enable moneyPerDay you need to activate the daytime captionName.");
                    conf.daytimeActive = true;
                    checkBox4.Checked = true;
                }
            }
        }

        private void checkBox22_CheckedChanged(object sender, EventArgs e)
        {
            conf.charactersLinkInSidebar = checkBox22.Checked;
        }

        private void generateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Generator.generateTwee2(conf);
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            if ((listView4.SelectedItems.Count == 1) && (listView8.SelectedItems.Count == 1))
            {
                // Get the right job
                bool brk = false;
                for (int i = 0; i < conf.jobs.Count; i++)
                {
                    if (conf.jobs[i].ID == int.Parse(listView4.SelectedItems[0].Text))
                    {
                        for (int j = 0; j < conf.jobs[i].rewardItems.Count; j++)
                        {
                            if (conf.jobs[i].rewardItems[j].ID == int.Parse(listView8.SelectedItems[0].SubItems[1].Text))
                            {
                                conf.jobs[i].rewardItems[j].type = comboBox7.Text;
                                conf.jobs[i].rewardItems[j].ID = int.Parse(comboBox4.Text);
                                conf.jobs[i].rewardItems[j].amount = Convert.ToInt32(numericUpDown13.Value);
                                listView8.SelectedItems[0].SubItems[0].Text = comboBox7.Text;
                                listView8.SelectedItems[0].SubItems[1].Text = comboBox4.Text;
                                listView8.SelectedItems[0].SubItems[2].Text = numericUpDown13.Value.ToString();
                                brk = true;
                                break;
                            }
                        }
                        if (brk) break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a job to update its items.");
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            if (listView4.SelectedItems.Count == 1)
            {
                // Get active job by id
                int activeJobId = -1;
                for (int i = 0; i < conf.jobs.Count; i++)
                {
                    if (conf.jobs[i].ID == int.Parse(listView4.SelectedItems[0].Text))
                    {
                        activeJobId = i;
                        break;
                    }
                }

                if (activeJobId == -1)
                {
                    MessageBox.Show("Could not find active job.");
                    return;
                }



                // Has already an item of that kind?
                bool itemAlreadyInList = false;
                for (int i = 0; i < conf.jobs[activeJobId].rewardItems.Count; i++)
                {
                    if (conf.jobs[activeJobId].rewardItems[i].ID == int.Parse(comboBox4.Text))
                    {
                        itemAlreadyInList = true;
                        break;
                    }
                }

                if (itemAlreadyInList)
                {
                    MessageBox.Show("Item is already in job reward list. Please update.");
                }
                else
                {
                    conf.jobs[activeJobId].rewardItems.Add(new RewardItem(comboBox7.Text, int.Parse(comboBox4.Text), Convert.ToInt32(numericUpDown13.Value)));
                    ListViewItem item = new ListViewItem();
                    item.Text = comboBox7.Text;
                    item.SubItems.Add(comboBox4.Text);
                    item.SubItems.Add(Convert.ToInt32(numericUpDown13.Value).ToString());
                    listView8.Items.Add(item);
                }
            }
            else
            {
                MessageBox.Show("Please select a job first.");
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if ((listView4.SelectedItems.Count == 1) && (listView8.SelectedItems.Count == 1))
            {
                // Get active job by id
                int activeJobId = -1;
                for (int i = 0; i < conf.jobs.Count; i++)
                {
                    if (conf.jobs[i].ID == int.Parse(listView4.SelectedItems[0].SubItems[0].Text))
                    {
                        activeJobId = i;
                        break;
                    }
                }

                if (activeJobId == -1)
                {
                    MessageBox.Show("Could not find active job.");
                    return;
                }

                for (int i = 0; i < conf.jobs[activeJobId].rewardItems.Count; i++)
                {
                    if (conf.jobs[activeJobId].rewardItems[i].ID == int.Parse(listView8.SelectedItems[0].SubItems[1].Text))
                    {
                        conf.jobs[activeJobId].rewardItems.RemoveAt(i);
                        listView8.Items.Remove(listView8.SelectedItems[0]);
                        break;
                    }
                }
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox7.Text.ToUpper().Equals("ITEM"))
            {
                for (int i = 0; i < conf.items.Count; i++)
                {
                    if (comboBox4.Text == conf.items[i].ID.ToString())
                    {
                        label80.Text = conf.items[i].name;

                        if (File.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.INVENTORY_PATH_PREFIX, conf.items[i].image)))
                        {
                            pictureBox2.Load(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.INVENTORY_PATH_PREFIX, conf.items[i].image));
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < conf.clothing.Count; i++)
                {
                    if (comboBox4.Text == conf.clothing[i].ID.ToString())
                    {
                        label80.Text = conf.clothing[i].name;

                        if (File.Exists(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, conf.clothing[i].image)))
                        {
                            pictureBox2.Load(Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, conf.clothing[i].image));
                        }
                    }
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.patreon.com/padmalcom");
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "|*.*||*.png||*.jpg||*.gif";
            openFileDialog1.Title = "Open Image File";
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                textBox9.Text = Path.GetFileName(openFileDialog1.FileName);
            }
        }

        private void checkBox17_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button30_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Generator.generateTwee2(conf)))
            {
                MessageBox.Show("Success!");
            }
        }

        private void checkBox20_CheckedChanged(object sender, EventArgs e)
        {
            conf.inventoryInSidebar = checkBox20.Checked;
        }

        private void checkBox21_CheckedChanged(object sender, EventArgs e)
        {
            conf.clothingInSidebar = checkBox21.Checked;
        }

        private void checkBox23_CheckedChanged(object sender, EventArgs e)
        {
            conf.statsInSidebar = checkBox23.Checked;
        }

        private void checkBox24_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("captionName", "captionName");
            dataGridView1.Columns.Add("caption", "caption");
            for (int i = 0; i < conf.captions.Count; i++)
            {
                var index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells["captionName"].Value = conf.captions[i].captionName;
                dataGridView1.Rows[index].Cells["caption"].Value = conf.captions[i].caption;
            }
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            comboBox6.SelectedIndex = comboBox6.FindStringExact("Item");
            comboBox7.SelectedIndex = comboBox7.FindStringExact("Item");

            checkedListBox3.Items.Clear();
            for (int i = 0; i < listView1.Columns.Count; i++)
            {
                checkedListBox3.Items.Add(listView1.Columns[i].Text);
                checkedListBox3.SetItemChecked(checkedListBox3.Items.Count - 1, true);
            }

            checkedListBox4.Items.Clear();
            for (int i = 0; i < listView2.Columns.Count; i++)
            {
                checkedListBox4.Items.Add(listView2.Columns[i].Text);
                checkedListBox4.SetItemChecked(checkedListBox4.Items.Count - 1, true);
            }

            checkedListBox2.Items.Clear();
            for (int i = 0; i < listView2.Columns.Count; i++)
            {
                checkedListBox2.Items.Add(listView2.Columns[i].Text);
                checkedListBox2.SetItemChecked(checkedListBox2.Items.Count - 1, true);
            }

            checkedListBox5.Items.Clear();
            for (int i = 0; i < listView7.Columns.Count; i++)
            {
                checkedListBox5.Items.Add(listView7.Columns[i].Text);
                checkedListBox5.SetItemChecked(checkedListBox5.Items.Count - 1, true);
            }

            checkedListBox6.Items.Clear();
            for (int i = 0; i < listView4.Columns.Count; i++)
            {
                checkedListBox6.Items.Add(listView4.Columns[i].Text);
                checkedListBox6.SetItemChecked(checkedListBox6.Items.Count - 1, true);
            }
            checkedListBox6.Items.Add("Reward Items");
            checkedListBox6.SetItemChecked(checkedListBox6.Items.Count - 1, true);

            for(int i=0; i<checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }

            // Make the pro version a free version
            if (!TweeFlyPro.Properties.Settings.Default.IsProEdition)
            {
                tabControl1.TabPages.Remove(tabPage2);
                tabControl1.TabPages.Remove(tabPage3);
                tabControl1.TabPages.Remove(tabPage4);
                tabControl1.TabPages.Remove(tabPage7);
                tabControl1.TabPages.Remove(tabPage8);
                tabControl1.TabPages.Remove(tabPage9);

                // Remove CLOTHING type from shops
                comboBox6.Items.Remove("CLOTHING");

                // Change logo
                pictureBox1.Image = TweeFlyPro.Properties.Resources.TweeFlyFreeLogo;
            }

            // Create projects folder if not exists
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "projects"))) {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "projects"));
            }

            Welcome welcomeForm = new Welcome(this);
            welcomeForm.ShowDialog();
        }

        private void checkBox25_CheckedChanged(object sender, EventArgs e)
        {
            conf.inventoryUseSkill1 = checkBox25.Checked;
        }

        private void checkBox26_CheckedChanged(object sender, EventArgs e)
        {
            conf.inventoryUseSkill2 = checkBox26.Checked;
        }

        private void checkBox27_CheckedChanged(object sender, EventArgs e)
        {
            conf.inventoryUseSkill3 = checkBox27.Checked;
        }

        private void checkBox28_CheckedChanged(object sender, EventArgs e)
        {
            conf.clothingUseSkill1 = checkBox28.Checked;
        }

        private void checkBox29_CheckedChanged(object sender, EventArgs e)
        {
            conf.clothingUseSkill2 = checkBox29.Checked;
        }

        private void checkBox30_CheckedChanged(object sender, EventArgs e)
        {
            conf.clothingUseSkill3 = checkBox30.Checked;
        }

        private void checkBox33_CheckedChanged(object sender, EventArgs e)
        {
            conf.characterUseSkill1 = checkBox33.Checked;
        }

        private void checkBox32_CheckedChanged(object sender, EventArgs e)
        {
            conf.characterUseSkill2 = checkBox32.Checked;
        }

        private void checkBox31_CheckedChanged(object sender, EventArgs e)
        {
            conf.characterUseSkill3 = checkBox31.Checked;
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox6.Text.Equals("Item"))
            {
                comboBox1.Items.Clear();
                for (int i = 0; i < conf.items.Count; i++)
                {
                    comboBox1.Items.Add(conf.items[i].ID);
                }
            }
            else
            {
                comboBox1.Items.Clear();
                for (int i = 0; i < conf.clothing.Count; i++)
                {
                    comboBox1.Items.Add(conf.clothing[i].ID);
                }
            }
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox7.Text.Equals("Item"))
            {
                comboBox4.Items.Clear();
                for (int i = 0; i < conf.items.Count; i++)
                {
                    comboBox4.Items.Add(conf.items[i].ID);
                }
            }
            else
            {
                comboBox4.Items.Clear();
                for (int i = 0; i < conf.clothing.Count; i++)
                {
                    comboBox4.Items.Add(conf.clothing[i].ID);
                }
            }
        }

        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {
            conf.startDate = dateTimePicker3.Value;
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            conf.itemPropertiesInShops.Clear();
            for(int i=0; i<checkedListBox1.CheckedItems.Count; i++)
            {
                conf.itemPropertiesInShops.Add(checkedListBox1.CheckedItems[i].ToString());
            }
        }

        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void checkedListBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            conf.displayInInventory.Clear();
            for (int i = 0; i < checkedListBox3.CheckedItems.Count; i++)
            {
                conf.displayInInventory.Add(checkedListBox3.CheckedItems[i].ToString());
            }
        }

        private void checkedListBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            conf.displayInWardrobe.Clear();
            for (int i = 0; i < checkedListBox4.CheckedItems.Count; i++)
            {
                conf.displayInWardrobe.Add(checkedListBox4.CheckedItems[i].ToString());
            }
        }

        private void checkedListBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            conf.displayInCharactersView.Clear();
            for (int i = 0; i < checkedListBox5.CheckedItems.Count; i++)
            {
                conf.displayInCharactersView.Add(checkedListBox5.CheckedItems[i].ToString());
            }
        }

        private void button31_Click(object sender, EventArgs e)
        {
            conf = new Configuration(true);

            // Global
            conf.inventoryActive = true;
            conf.clothingActive = true;
            conf.statsActive = true;
            conf.daytimeActive = true;
            conf.shopActive = true;
            conf.moneyActive = true;
            conf.jobsActive = true;
            conf.charactersActive = true;
            conf.storyName = "The Atlantic Murder";

            // Inventory
            conf.inventoryInSidebar = true;
            conf.inventoryLinkInSidebar = true;
            conf.inventorySidebarTooltip = true;
            conf.inventoryUseSkill1 = false;
            conf.inventoryUseSkill2 = false;
            conf.inventoryUseSkill3 = false;

            conf.items.Add(new Item(0, "taxi driver card", "The card from the taxi driver I met when I arrived here.", false, "story item",
                "", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.INVENTORY_PATH_PREFIX, "taxicard.jpg"), -1, -1, false, 0, "", "", "", ""));
            conf.items.Add(new Item(1, "smartphone", "My smartphone I got from my mom", false, "story item", "", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.INVENTORY_PATH_PREFIX, "smartphone.jpg"), -1, -1,
                false, 1, "", "", "", ""));
            conf.items.Add(new Item(2, "my keycard", "The keycard to my room in the hotel", false, "story item", "", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.INVENTORY_PATH_PREFIX, "keycard.jpg"), -1, -1,
                false, 0, "", "", "", ""));
            conf.items.Add(new Item(3, "Chocolate", "A chocolate bar", true, "food", "", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.INVENTORY_PATH_PREFIX, "chocolate.jpg"), 2, 1,
                true, 1, "", "", "", ""));

            conf.displayInInventory.Clear();
            /*for (int i = 0; i < checkedListBox3.CheckedItems.Count; i++)
            {
                conf.displayInInventory.Add(checkedListBox3.CheckedItems[i].ToString());
            }*/
            conf.displayInInventory.Add("Name");
            conf.displayInInventory.Add("Description");
            conf.displayInInventory.Add("Owned");
            conf.displayInInventory.Add("Image");

            // Clothing
            conf.clothingInSidebar = true;
            conf.clothingLinkInSidebar = true;
            conf.clothingUseSkill1 = false;
            conf.clothingUseSkill2 = false;
            conf.clothingUseSkill3 = false;

            conf.displayInWardrobe.Clear();
            /*for (int i = 0; i < checkedListBox4.CheckedItems.Count; i++)
            {
                conf.displayInWardrobe.Add(checkedListBox4.CheckedItems[i].ToString());
            }*/
            conf.displayInWardrobe.Add("Name");
            conf.displayInWardrobe.Add("Description");
            conf.displayInWardrobe.Add("Body part");
            conf.displayInWardrobe.Add("Owned");
            conf.displayInWardrobe.Add("Image");

            conf.displayInClothingView.Clear();
            /*for (int i = 0; i < checkedListBox2.CheckedItems.Count; i++)
            {
                conf.displayInClothingView.Add(checkedListBox2.CheckedItems[i].ToString());
            }*/
            conf.displayInClothingView.Add("Name");
            conf.displayInClothingView.Add("Body part");
            conf.displayInClothingView.Add("Owned");
            conf.displayInClothingView.Add("Image");

            // name, description, canbebougt, shopcat, cat, bodypart, image, buiprice, sellprice, multiple, owned, s1, s2, s3
            conf.clothing.Add(new Clothing(0, "nothing", "description", false, "", "clothing", "HEAD_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "none.jpg"), -1, -1, false, 1, "", "", "", true));
            conf.clothing.Add(new Clothing(1, "beanie", "description", true, "", "clothing", "HEAD_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "beanie.jpg"), 20, 10, true, 1, "", "", "", false));

            // Hair
            conf.clothing.Add(new Clothing(2, "nothing", "description",false, "", "clothing", "HAIR_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "none.jpg"), -1, -1, false, 1, "", "", "", true));
            conf.clothing.Add(new Clothing(3, "headscarf", "description",true, "", "clothing", "HAIR_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "headscarf.jpg"), 20, 10, true, 1, "", "", "", false));

            // Neck
            conf.clothing.Add(new Clothing(4, "nothing", "description",false,"", "clothing", "NECK_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "none.jpg"), -1, -1, false, 1, "", "", "", true));
            conf.clothing.Add(new Clothing(5, "necklace (male)", "description",true, "", "clothing", "NECK_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "necklace_male.jpg"), 20, 10, true, 1, "", "", "", false));

            // Upper body
            conf.clothing.Add(new Clothing(6, "nothing", "description",false, "", "clothing","UPPER_BODY_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "none.jpg"), -1, -1, false, 1, "", "", "", false));
            conf.clothing.Add(new Clothing(7, "shirt", "description",true,"", "clothing",  "UPPER_BODY_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "shirt.jpg"), 20, 10, true, 1, "", "", "", false));
            conf.clothing.Add(new Clothing(8, "tshirt", "description",true, "", "clothing", "UPPER_BODY_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "tshirt.jpg"), 20, 10, true, 1, "", "", "", true));

            // Lower body
            conf.clothing.Add(new Clothing(9, "nothing", "description",false,"","clothing", "LOWER_BODY_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "none.jpg"), -1, -1, false, 1, "", "", "", false));
            conf.clothing.Add(new Clothing(10, "jeans", "description",true, "", "clothing", "LOWER_BODY_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "jeans.jpg"), 20, 10, true, 1, "", "", "", true));
            conf.clothing.Add(new Clothing(11, "short pants", "description",true, "", "clothing", "LOWER_BODY_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "shortpants.jpg"), 20, 10, true, 1, "", "", "", false));

            // Belt
            conf.clothing.Add(new Clothing(12, "nothing", "description",false, "", "clothing",  "BELT_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "none.jpg"), -1, -1, false, 1, "", "", "", true));
            conf.clothing.Add(new Clothing(13, "simple belt", "description",true, "", "clothing", "BELT_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "belt.jpg"), 20, 10, true, 1, "", "", "", false));

            // Socks
            conf.clothing.Add(new Clothing(14, "nothing", "description",false,"", "clothing", "SOCKS_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "none.jpg"), -1, -1, false, 1, "", "", "", false));
            conf.clothing.Add(new Clothing(15, "male socks", "description",true, "", "clothing", "SOCKS_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "socks.jpg"), 20, 10, true, 1, "", "", "", true));

            // Shoes
            conf.clothing.Add(new Clothing(16, "nothing", "description",false, "", "clothing","SHOES_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "none.jpg"), -1, -1, false, 1, "", "", "", false));
            conf.clothing.Add(new Clothing(17, "shoes (male)", "description",true, "", "clothing", "SHOES_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "shoes_male.jpg"), 20, 10, true, 1, "", "", "", true));

            // Underwear top
            conf.clothing.Add(new Clothing(18, "nothing", "description",false,"","clothing", "UNDERWEAR_TOP_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "none.jpg"), -1, -1, false, 1, "", "", "", true));
            conf.clothing.Add(new Clothing(19, "tanktop", "description",true,"","clothing","UNDERWEAR_TOP_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "tanktop.jpg"), 20, 10, true, 1, "", "", "", false));

            // Underwear bottom
            conf.clothing.Add(new Clothing(20, "nothing", "description",false, "", "clothing", "UNDERWEAR_BOTTOM_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "none.jpg"), -1, -1, false, 1, "", "", "", false));
            conf.clothing.Add(new Clothing(21, "boxershorts", "description",true,"", "clothing", "UNDERWEAR_BOTTOM_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "boxershorts.jpg"), 20, 10, true, 1, "", "", "", true));

            // Suit
            conf.clothing.Add(new Clothing(22, "Suit pants", "description", true, "", "clothing", "LOWER_BODY_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "suit.jpg"), 2000, -1, false, 1, "", "", "", false));
            conf.clothing.Add(new Clothing(23, "Suit shirt", "description", true, "", "clothing", "UPPER_BODY_NAME", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CLOTHING_PATH_PREFIX, "suit.jpg"), 2000, -1, false, 1, "", "", "", false));


            // Stats
            conf.statsInSidebar = true;
            conf.statsLinkInSidebar = true;
            conf.stats.Add(new Stats(0, "strength", "my strength", "0", "", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.STATS_PATH_PREFIX, "strength.jpg"), true, "None"));
            conf.stats.Add(new Stats(0, "intelligence", "my intelligence", "0", "", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.STATS_PATH_PREFIX, "intelligence.jpg"), true, "None"));

            // Daytime
            conf.daytimeFormat = 0;
            conf.daytimeInSidebar = true;
            conf.startDate = new DateTime(2015, 12, 12, 20, 20, 00);

            // Shops
            Shop s1 = new Shop(0, "gift shop", new DateTime(2000, 01, 01, 0, 0, 0), new DateTime(2000, 01, 01, 23, 59, 59));
            s1.items.Add(new ShopItem("CLOTHING", 13, 10, 10, 1));
            s1.items.Add(new ShopItem("CLOTHING", 21, 1, 1, 0));
            s1.items.Add(new ShopItem("ITEM", 3, 1, 1, 1));
            conf.shops.Add(s1);
            conf.itemPropertiesInShops.Clear();
            for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
            {
                conf.itemPropertiesInShops.Add(checkedListBox1.CheckedItems[i].ToString());
            }

            // Money
            conf.moneyInSidebar = true;
            conf.moneyPerDay = 1;
            conf.startMoney = 10;

            // Jobs
            Job j1 = new Job(0, "wash dishes", "washing the dishes", true, 180, "kitchen", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.JOBS_PATH_PREFIX, "dishes.jpg"), 5, 60, "");
            Job j2 = new Job(1, "clean floor", "cleaning the kitchen floor", true, 330, "kitchen", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.JOBS_PATH_PREFIX, "cleaningfloor.jpg"), 10, 120, "");
            j1.rewardItems.Add(new RewardItem("ITEM", 3, 1));
            conf.jobs.Add(j1);
            conf.jobs.Add(j2);
            conf.displayInJobsView.Clear();
            /*for (int i = 0; i < checkedListBox6.CheckedItems.Count; i++)
            {
                conf.displayInJobsView.Add(checkedListBox6.CheckedItems[i].ToString());
            }*/
            conf.displayInJobsView.Add("Name");
            conf.displayInJobsView.Add("Description");
            conf.displayInJobsView.Add("Reward money");
            conf.displayInJobsView.Add("Cooldown");
            conf.displayInJobsView.Add("Image");

            // Characters
            Character c0 = new Character(0, "Player", 21, "You", true, "", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CHARACTERS_PATH_PREFIX, "player.jpg"), "male", "", 0, "#42f46e", "", "", "");
            Character c1 = new Character(1, "Receptionist", 53, "Arthur is a receptionist.", false, "hotel", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CHARACTERS_PATH_PREFIX, "jim.jpg"), "male", "receptionist", 0, "#42f44b", "", "", "");
            Character c2 = new Character(2, "Footboy", 21, "Mike is a footboy.", false, "hotel", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CHARACTERS_PATH_PREFIX, "rex.jpg"), "male", "footboy", 0, "#f44242", "", "", "");
            Character c3 = new Character(3, "Taxi driver", 50, "", false, "", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CHARACTERS_PATH_PREFIX, "simon.jpg"), "male", "", 0, "#f44242", "", "", "");
            Character c4 = new Character(4, "Woman", 50, "", false, "", Path.Combine(projectDir, Generator.IMAGE_PATH_PREFIX, Generator.CHARACTERS_PATH_PREFIX, "joana.jpg"), "female", "", 0, "#f4d942", "", "", "");
            conf.characters.Add(c0);
            conf.characters.Add(c1);
            conf.characters.Add(c2);
            conf.characters.Add(c3);
            conf.characters.Add(c4);
            conf.charactersInSidebar = true;
            conf.charactersLinkInSidebar = true;
            conf.charactersSidebarTooltip = true;
            conf.characterUseSkill1 = false;
            conf.characterUseSkill2 = false;
            conf.characterUseSkill3 = false;

            conf.displayInCharactersView.Clear();
            /*for (int i = 0; i < checkedListBox5.CheckedItems.Count; i++)
            {
                conf.displayInCharactersView.Add(checkedListBox5.CheckedItems[i].ToString());
            }*/
            conf.displayInCharactersView.Add("Name");
            conf.displayInCharactersView.Add("Description");
            conf.displayInCharactersView.Add("Age");
            conf.displayInCharactersView.Add("Gender");
            conf.displayInCharactersView.Add("Job");
            conf.displayInCharactersView.Add("Relation");
            conf.displayInCharactersView.Add("Image");


            // CSS
            conf.resizeImagesInSidebar = true;
            conf.imageWidthInSidebar = 50;
            conf.imageHeightInSidebar = 50;

            conf.resizeImagesInParagraph = true;
            conf.imageWidthInParagraph = 80;
            conf.imageHeightInParagraph = 80;

            conf.resizeImagesInDialogs = true;
            conf.imageWidthInDialogs = 80;
            conf.imageHeightInDialogs = 80;


            updateFromConf(conf);
        }

        private void checkBox34_CheckedChanged(object sender, EventArgs e)
        {
            conf.wardrobeLinkInSidebar = checkBox34.Checked;
        }

        private void listView3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkedListBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            conf.displayInJobsView.Clear();
            for (int i = 0; i < checkedListBox6.CheckedItems.Count; i++)
            {
                conf.displayInJobsView.Add(checkedListBox6.CheckedItems[i].ToString());
            }
        }

        private void listView8_DoubleClick(object sender, EventArgs e)
        {
            if (listView8.SelectedItems.Count == 1)
            {
                comboBox7.Text = listView8.SelectedItems[0].SubItems[0].Text;
                comboBox4.Text = listView8.SelectedItems[0].SubItems[1].Text;
                numericUpDown13.Value = int.Parse(listView8.SelectedItems[0].SubItems[2].Text);
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            conf.daytimeFormat = 3;
        }

        private void checkBox36_CheckedChanged(object sender, EventArgs e)
        {
            conf.charactersInSidebar = checkBox36.Checked;
        }

        private void checkBox39_CheckedChanged(object sender, EventArgs e)
        {
            conf.shopUseSkill1 = checkBox39.Checked;
        }

        private void checkBox38_CheckedChanged(object sender, EventArgs e)
        {
            conf.shopUseSkill2 = checkBox38.Checked;
        }

        private void checkBox37_CheckedChanged(object sender, EventArgs e)
        {
            conf.shopUseSkill3 = checkBox37.Checked;
        }

        private void checkedListBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            conf.displayInClothingView.Clear();
            for (int i = 0; i < checkedListBox2.CheckedItems.Count; i++)
            {
                conf.displayInClothingView.Add(checkedListBox2.CheckedItems[i].ToString());
            }
        }

        private void button32_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            DialogResult dr = cd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                panel1.BackColor = cd.Color;
            }
        }

        public static String HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        private void button33_Click(object sender, EventArgs e)
        {
            string savedPath = Generator.generateTwee2(conf);
            if (!string.IsNullOrEmpty(savedPath))
            {
                if (ExistsOnPath("twee2"))
                {
                    string storyFile = string.IsNullOrEmpty(conf.storyName) ? "story.tw2" : conf.storyName;
                    storyFile = storyFile.EndsWith(".tw2") ? storyFile : storyFile + ".tw2";
                    string htmlFile = storyFile.Remove(storyFile.Length - 4, 4) + ".html";

                    storyFile = Path.Combine(savedPath, storyFile);
                    htmlFile = Path.Combine(savedPath, htmlFile);

                    ProcessStartInfo _processStartInfo = new ProcessStartInfo();
                    _processStartInfo.WorkingDirectory = savedPath;
                    _processStartInfo.FileName = @"twee2";
                    _processStartInfo.Arguments = "build \"" + storyFile + "\" \"" + htmlFile + "\"";
                    Process.Start(_processStartInfo);
                } else
                {
                    MessageBox.Show("twee2 is not found.");
                }
            }
        }

        // Check if twee2 can be executed
        private static bool ExistsOnPath(string fileName)
        {
            return GetFullPath(fileName) != null;
        }

        private static string GetFullPath(string fileName)
        {
            if (File.Exists(fileName))
                return Path.GetFullPath(fileName);

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(';'))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                    return fullPath;
            }
            return null;
        }

        private void checkBox40_CheckedChanged(object sender, EventArgs e)
        {
            conf.navigationArrows = checkBox40.Checked;
        }

        private void checkBox41_CheckedChanged(object sender, EventArgs e)
        {
            conf.debugMode = checkBox41.Checked;
        }

        private void checkBox43_CheckedChanged(object sender, EventArgs e)
        {
            conf.resizeImagesInSidebar = checkBox43.Checked;
            numericUpDown26.Enabled = checkBox43.Checked;
            numericUpDown27.Enabled = checkBox43.Checked;
        }

        private void numericUpDown26_ValueChanged(object sender, EventArgs e)
        {
            conf.imageWidthInSidebar = Convert.ToInt32(numericUpDown26.Value);
        }

        private void numericUpDown27_ValueChanged(object sender, EventArgs e)
        {
            conf.imageHeightInSidebar = Convert.ToInt32(numericUpDown27.Value);
        }

        private void checkBox42_CheckedChanged(object sender, EventArgs e)
        {
            conf.resizeImagesInParagraph = checkBox42.Checked;
            numericUpDown29.Enabled = checkBox42.Checked;
            numericUpDown28.Enabled = checkBox42.Checked;
        }

        private void numericUpDown29_ValueChanged(object sender, EventArgs e)
        {
            conf.imageWidthInParagraph = Convert.ToInt32(numericUpDown29.Value);
        }

        private void numericUpDown28_ValueChanged(object sender, EventArgs e)
        {
            conf.imageHeightInParagraph = Convert.ToInt32(numericUpDown28.Value);
        }

        private void checkBox44_CheckedChanged(object sender, EventArgs e)
        {
            conf.resizeImagesInDialogs = checkBox44.Checked;
            numericUpDown31.Enabled = checkBox44.Checked;
            numericUpDown24.Enabled = checkBox44.Checked;
        }

        private void numericUpDown31_ValueChanged(object sender, EventArgs e)
        {
            conf.imageWidthInDialogs = Convert.ToInt32(numericUpDown31.Value);
        }

        private void numericUpDown24_ValueChanged(object sender, EventArgs e)
        {
            conf.imageHeightInDialogs = Convert.ToInt32(numericUpDown24.Value);
        }

        private void numericUpDown30_ValueChanged(object sender, EventArgs e)
        {
            conf.paragraphWidth = Convert.ToInt32(numericUpDown30.Value);
        }

        private void checkBox48_CheckedChanged(object sender, EventArgs e)
        {
            conf.charactersSidebarTooltip = checkBox48.Checked;
        }

        private void checkBox45_CheckedChanged(object sender, EventArgs e)
        {
            conf.inventorySidebarTooltip = checkBox45.Checked;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            textBox10.Tag = textBox10.Text;
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            textBox11.Tag = textBox11.Text;
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            textBox9.Tag = textBox9.Text;
        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {
            textBox15.Tag = textBox15.Text;
        }

        private void textBox37_TextChanged(object sender, EventArgs e)
        {
            textBox37.Tag = textBox37.Text;
        }

        private void textBox47_TextChanged(object sender, EventArgs e)
        {
            conf.storyName = textBox47.Text;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About f = new About();
            f.ShowDialog(this);
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            for(int i=0; i<conf.captions.Count; i++)
            {
                if (conf.captions[i].captionName.Equals(dataGridView1[0, e.RowIndex].Value.ToString()))
                {
                    conf.captions[i].caption = dataGridView1[1, e.RowIndex].Value.ToString();
                    break;
                }
            }
        }

        private void button34_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Generator.generateTwine(conf)))
            {
                MessageBox.Show("Success!");
            }
        }

        private void button35_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "|*.html|*.*|";
            saveFileDialog1.Title = "TweeFly Cheat Sheet";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                TextWriter twCheatSheet = null;
                try
                {
                    twCheatSheet = new StreamWriter(saveFileDialog1.FileName, false, new UTF8Encoding(false));

                    twCheatSheet.WriteLine("<html>\n<title>TweeFly Cheat Sheet for " + conf.storyName + "</title><body>\n");
                    twCheatSheet.WriteLine("<style>table {font-family: arial, sans-serif; border-collapse: collapse; width: 100%;}");
                    twCheatSheet.WriteLine("td, th {border: 1px solid #dddddd; text-align: left; padding: 8px;}");
                    twCheatSheet.WriteLine("tr: nth-child(even) {background-color: #dddddd;}</style>");
                    twCheatSheet.WriteLine("<h1>TweeFly Cheat Sheet for " + conf.storyName + "<h1>\n");

                    twCheatSheet.WriteLine("<h2>Initialization code (no need to call)<h2>\n");
                    // Initialization

                    twCheatSheet.WriteLine("<table><tr><td style=\"width:15%\"><b>Function</b></td><td style=\"width:7%\"><b>Type</b></td><td style=\"width:28%\"><b>Parameters</b></td><td style=\"width: 50%\"><b>Description</b></td></tr>\n");

                    // - cloth
                    twCheatSheet.WriteLine("<tr><td>initAllClothing</td><td>Macro</td><td>-</td><td>Initialized global clothing list</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>initClothing</td><td>Macro</td><td>-</td><td>Initialized the worn clothing.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>initWardrobe</td><td>Macro</td><td>-</td><td>Initialized all clothing in player's wardrobe.</td></tr>\n");

                    // - inventory
                    twCheatSheet.WriteLine("<tr><td>initItems</td><td>Macro</td><td>-</td><td>Initialized all global items.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>initInventory</td><td>Macro</td><td>-</td><td>Initialized all items in player's inventory.</td></tr>\n");
                  
                    // - general
                    twCheatSheet.WriteLine("<tr><td>initMoney</td><td>Macro</td><td>-</td><td>Initializes money system.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>initStats</td><td>Macro</td><td>-</td><td>Initializes stats system.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>initJobs</td><td>Macro</td><td>-</td><td>Initializes job system.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>initCharacters</td><td>Macro</td><td>-</td><td>Initializes character system.</td></tr></table>\n");

                    // Display functions
                    twCheatSheet.WriteLine("<h2>Paragraph views<h2>\n");
                    twCheatSheet.WriteLine("<table><tr><td style=\"width:15%\"><b>Function</b></td><td style=\"width:7%\"><b>Type</b></td><td style=\"width:28%\"><b>Parameters</b></td><td style=\"width: 50%\"><b>Description</b></td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>inventory</td><td>Macro</td><td>-</td><td>Show the inventory.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>variables</td><td>Macro</td><td>-</td><td>Show all variables used in this game (for debug purposes)</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>clothing</td><td>Macro</td><td>-</td><td>Show all worn clothes.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>wardrobe</td><td>Macro</td><td>-</td><td>Show the player's wardrobe.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>stats</td><td>Macro</td><td>-</td><td>Show the player's stats.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>characters</td><td>Macro</td><td>-</td><td>Show all characters of the game.</td></tr></table>\n");

                    // Sidebar boxes
                    twCheatSheet.WriteLine("<h2>Sidebar views<h2>\n");
                    twCheatSheet.WriteLine("<table><tr><td style=\"width:15%\"><b>Function</b></td><td style=\"width:7%\"><b>Type</b></td><td style=\"width:28%\"><b>Parameters</b></td><td style=\"width: 50%\"><b>Description</b></td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>inventorySidebar</td><td>Macro</td><td>-</td><td>Shows the player's inventory.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>clothingSidebar</td><td>Macro</td><td>-</td><td>Shows the player's clothing.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>statsSidebar</td><td>Macro</td><td>-</td><td>Shows the player's stats.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>charactersSidebar</td><td>Macro</td><td>-</td><td>Shows all known characters.</td></tr></table>\n");

                    // Time
                    twCheatSheet.WriteLine("<h2>Time functions<h2>\n");
                    twCheatSheet.WriteLine("<table><tr><td style=\"width:15%\"><b>Function</b></td><td style=\"width:7%\"><b>Type</b></td><td style=\"width:28%\"><b>Parameters</b></td><td style=\"width: 50%\"><b>Description</b></td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>getTime</td><td>Macro</td><td>-</td><td>Get the current time in the game.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>getDate</td><td>Macro</td><td>-</td><td>Get the current date in the game.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>getDateTime</td><td>Macro</td><td>-</td><td>Get the current time and date in the game.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>getDaysPassed</td><td>Macro</td><td>-</td><td>Get the days that passed since starting the game.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>setTime</td><td>Macro</td><td>hours, minutes, seconds</td><td>Set the time.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>setDate</td><td>Macro</td><td>year, month, day</td><td>Set the date.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>setDateTime</td><td>Macro</td><td>year, month, day, hours, minutes, seconds</td><td>Set date and time.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>addTimeInMinutes</td><td>Macro</td><td>minutes</td><td>Add time in minutes.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>addTimeInDays</td><td>Macro</td><td>days</td><td>Add time in days.</td></tr></table>\n");

                    // Inventory
                    twCheatSheet.WriteLine("<h2>Inventory<h2>\n");
                    twCheatSheet.WriteLine("<table><tr><td style=\"width:15%\"><b>Function</b></td><td style=\"width:7%\"><b>Type</b></td><td style=\"width:28%\"><b>Parameters</b></td><td style=\"width: 50%\"><b>Description</b></td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>addToInventory</td><td>Macro</td><td>item id, amount</td><td>Adds the amount of items to the player's inventory.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>removeFromInventory</td><td>Macro</td><td>item id, amount (optional)</td><td>Removes the amount of items from the player's inventory.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>clearInventory</td><td>Macro</td><td>-</td><td>Removes all items from the player's inventory.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>showInventoryLink</td><td>Macro</td><td>-</td><td>Sets inventory link to visible.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>showInventorySidebar</td><td>Macro</td><td>-</td><td>Sets the sidebar inventory to visible.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>hideInventoryLink</td><td>Macro</td><td>-</td><td>Hides the inventory link.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>hideInventorySidebar</td><td>Macro</td><td>-</td><td>Hides the inventory from the sidebar.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>getInventory</td><td>Macro</td><td>-</td><td>Get all items from the inventory as array.</td></tr>\n");               
                    twCheatSheet.WriteLine("<tr><td>has_item</td><td>Macro</td><td>item id</td><td>Returns 1 if the player owns one or more of this item, otherwise: 0</td></tr></table>\n");

                    // Clothing
                    twCheatSheet.WriteLine("<h2>Clothing<h2>\n");
                    twCheatSheet.WriteLine("<table><tr><td style=\"width:15%\"><b>Function</b></td><td style=\"width:7%\"><b>Type</b></td><td style=\"width:28%\"><b>Parameters</b></td><td style=\"width: 50%\"><b>Description</b></td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>addToWardrobe</td><td>Macro</td><td>clothing id, amount</td><td>Adds the given amount of an item to the player's wardrobe.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>is_worn</td><td>Macro</td><td>clothing id</td><td>Is the player wearing a cloth? True if yes, False if not.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>is_naked</td><td>Macro</td><td>-</td><td>Is not wearing any cloths? True if yes, False if not.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>is_in_wardrobe</td><td>Macro</td><td>clothing id</td><td>Does the player own such cloth? True if yes, False if not.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>wear</td><td>Macro</td><td>clothing id</td><td>Wear the given cloth.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>undress</td><td>Macro</td><td>clothing id</td><td>Remove the given cloth.</td></tr></table>\n");

                    // Stats
                    twCheatSheet.WriteLine("<h2>Stats<h2>\n");
                    twCheatSheet.WriteLine("<table><tr><td style=\"width:15%\"><b>Function</b></td><td style=\"width:7%\"><b>Type</b></td><td style=\"width:28%\"><b>Parameters</b></td><td style=\"width: 50%\"><b>Description</b></td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>setStats</td><td>Macro</td><td>stats id, value</td><td>Sets the stats to the given value.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>addStats</td><td>Macro</td><td>stats id, value</td><td>Adds value to the given stats.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>setStatsVisible</td><td>Macro</td><td>stats id, boolean</td><td>Show of hide the skill from the skills view.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>getStats</td><td>Macro</td><td>stats id</td><td>Get's the stats or the sum if stats are bound to clothing.</td></tr></table>\n");

                    // Shops
                    twCheatSheet.WriteLine("<h2>Shops<h2>\n");
                    twCheatSheet.WriteLine("<table><tr><td style=\"width:15%\"><b>Function</b></td><td style=\"width:7%\"><b>Type</b></td><td style=\"width:28%\"><b>Parameters</b></td><td style=\"width: 50%\"><b>Description</b></td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>shop</td><td>Macro</td><td>shop id</td><td>Shows the shop with the given id.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>buy</td><td>Macro</td><td>item_object, type (ITEM or CLOTHING), shopid, _itemIndex of shop</td><td>Buy an item. This function call is generated from the shop view.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>sell</td><td>Macro</td><td>item_object, type (ITEM or CLOTHING), shopid, _itemIndex of shop</td><td>Sell an item. This function call is generated from the shop view.</td></tr></table>\n");

                    // Money
                    twCheatSheet.WriteLine("<h2>Money<h2>\n");
                    twCheatSheet.WriteLine("<table><tr><td style=\"width:15%\"><b>Function</b></td><td style=\"width:7%\"><b>Type</b></td><td style=\"width:28%\"><b>Parameters</b></td><td style=\"width: 50%\"><b>Description</b></td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>addMoney</td><td>Macro</td><td>amount</td><td>Adds the given amount of money.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>printMoney</td><td>Macro</td><td>-</td><td>Outputs the money in the player's wallet. Updates the amount when there is a pocket money.</td></tr></table>\n");

                    // Jobs
                    twCheatSheet.WriteLine("<h2>Jobs<h2>\n");
                    twCheatSheet.WriteLine("<table><tr><td style=\"width:15%\"><b>Function</b></td><td style=\"width:7%\"><b>Type</b></td><td style=\"width:28%\"><b>Parameters</b></td><td style=\"width: 50%\"><b>Description</b></td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>showJobs</td><td>Macro</td><td>Array of job ids</td><td>Prints all jobs with the given ids.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>doJob</td><td>Macro</td><td>job object</td><td>Does the given job. Updates cooldown and adds reward items and money.</td></tr></table>\n");

                    // Characters
                    twCheatSheet.WriteLine("<h2>Characters<h2>\n");
                    twCheatSheet.WriteLine("<table><tr><td style=\"width:15%\"><b>Function</b></td><td style=\"width:7%\"><b>Type</b></td><td style=\"width:28%\"><b>Parameters</b></td><td style=\"width: 50%\"><b>Description</b></td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>say</td><td>Macro</td><td>character id, text</td><td>Let the player say something.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>renameCharacter</td><td>Macro</td><td>character id, new name</td><td>Prints all jobs with the given ids.</td></tr>\n");
                    twCheatSheet.WriteLine("<tr><td>setKnown</td><td>Macro</td><td>character id</td><td>Sets the character to a known state so that his/her name is printed.</td></tr></table>\n");

                    twCheatSheet.WriteLine("</body></html>");

                }
                finally
                {
                    if (twCheatSheet != null)
                    {
                        twCheatSheet.Flush();
                        twCheatSheet.Close();
                    }
                }
            }
        }

        private void button36_Click(object sender, EventArgs e)
        {
            string newEntry = Interaction.InputBox("Enter a file name");
            if (!newEntry.Trim().Equals(""))
            {
                if (!listBox1.Items.Contains(newEntry))
                {
                    listBox1.Items.Add(newEntry);
                    conf.includedFiles.Clear();
                    for(int i=0; i<listBox1.Items.Count; i++)
                    {
                        conf.includedFiles.Add(listBox1.Items[i].ToString());
                    }
                }
            }
        }

        private void button37_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)
            {
                listBox1.Items.Remove(listBox1.SelectedItem);
            }
            conf.includedFiles.Clear();
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                conf.includedFiles.Add(listBox1.Items[i].ToString());
            }
        }

        private void checkBox46_CheckedChanged(object sender, EventArgs e)
        {
            conf.listVariables = checkBox46.Checked;
        }

        private void documentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            string directory = System.IO.Path.GetDirectoryName(path);
            directory = new Uri(directory).LocalPath;
            string[] files = Directory.GetFiles(directory, "TweeFly Documentation*.pdf");

            if (files.Length < 1)
            {
                MessageBox.Show("Documentation file not found.");
            } else
            {
                System.Diagnostics.Process.Start(files[0]);
            }
        }

        private void checkBox47_CheckedChanged(object sender, EventArgs e)
        {
            conf.storeProjectFileAsBinary = checkBox47.Checked;
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void openProjectsFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pf = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "projects");
            if (Directory.Exists(pf))
            {
                Process.Start("explorer.exe", pf);
            }
        }

        private void convertTfcToTfcxToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Get the new sorting column.
            ColumnHeader new_sorting_column = listView1.Columns[e.Column];

            // Figure out the new sorting order.
            System.Windows.Forms.SortOrder sort_order;
            if (SortingColumn == null)
            {
                // New column. Sort ascending.
                sort_order = SortOrder.Ascending;
            }
            else
            {
                // See if this is the same column.
                if (new_sorting_column == SortingColumn)
                {
                    // Same column. Switch the sort order.
                    if (SortingColumn.Text.StartsWith("> "))
                    {
                        sort_order = SortOrder.Descending;
                    }
                    else
                    {
                        sort_order = SortOrder.Ascending;
                    }
                }
                else
                {
                    // New column. Sort ascending.
                    sort_order = SortOrder.Ascending;
                }

                // Remove the old sort indicator.
                SortingColumn.Text = SortingColumn.Text.Substring(2);
            }

            // Display the new sort order.
            SortingColumn = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                SortingColumn.Text = "> " + SortingColumn.Text;
            }
            else
            {
                SortingColumn.Text = "< " + SortingColumn.Text;
            }

            // Create a comparer.
            listView1.ListViewItemSorter =
                new ListViewComparer(e.Column, sort_order);

            // Sort.
            listView1.Sort();
        }
    }
}
