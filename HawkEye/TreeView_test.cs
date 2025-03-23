using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HawkEye
{
    public partial class TreeView_test : Form
    {
        private TreeNode testUsernameNode;

        public TreeView_test()
        {
            InitializeComponent();
            InitializeTreeView();
            InitializeTreeViewDragAndDrop();
            load_setting();
            //this.FormClosing += new FormClosingEventHandler(TreeView_test_FormClosing);
            treeView1.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(treeView1_NodeMouseDoubleClick);
            treeView1.AfterLabelEdit += new NodeLabelEditEventHandler(treeView1_AfterLabelEdit);
        }

        // 設定情報の取得
        private void load_setting()
        {
            string test_username = Properties.Settings.Default.UserName;
            testUsernameNode = new TreeNode(test_username);
            treeView1.Nodes.Add(testUsernameNode);
        }

        //// フォーム終了時に設定を保存
        //private void TreeView_test_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //    Properties.Settings.Default.UserName = "new_username" + currentTime;
        //    Properties.Settings.Default.Save();
        //}

        private void InitializeTreeView()
        {
            // ノードを追加
            TreeNode rootNode1 = new TreeNode("Root Node1");
            rootNode1.Nodes.Add("Child Node 1");
            rootNode1.Nodes.Add("Child Node 2");

            TreeNode rootNode2 = new TreeNode("Root Node2");
            rootNode2.Nodes.Add("Child Node 1");
            rootNode2.Nodes.Add("Child Node 2");
            rootNode1.Nodes.Add(rootNode2);

            treeView1.Nodes.Add(rootNode1);

            // 全てのノードを展開
            treeView1.ExpandAll();
        }

        private void InitializeTreeViewDragAndDrop()
        {
            treeView1.AllowDrop = true;
            treeView1.ItemDrag += new ItemDragEventHandler(treeView1_ItemDrag);
            treeView1.DragEnter += new DragEventHandler(treeView1_DragEnter);
            treeView1.DragOver += new DragEventHandler(treeView1_DragOver);
            treeView1.DragDrop += new DragEventHandler(treeView1_DragDrop);
        }

        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeNode draggedNode = e.Item as TreeNode;
            if (draggedNode != null && draggedNode.Text != "Root Node1")
            {
                DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }

        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void treeView1_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                TreeNode newNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
                Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
                TreeNode targetNode = ((TreeView)sender).GetNodeAt(pt);

                if (targetNode != null && newNode != targetNode && !ContainsNode(newNode, targetNode))
                {
                    newNode.Remove();
                    if (targetNode.Parent == null)
                    {
                        treeView1.Nodes.Insert(targetNode.Index, newNode);
                    }
                    else
                    {
                        targetNode.Parent.Nodes.Insert(targetNode.Index, newNode);
                    }
                    newNode.EnsureVisible();
                }
            }
        }

        private bool ContainsNode(TreeNode node1, TreeNode node2)
        {
            if (node2.Parent == null) return false;
            if (node2.Parent == node1) return true;
            return ContainsNode(node1, node2.Parent);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Console.WriteLine("Selected Node: " + e.Node.Text);
        }

        // ノードのラベルを編集可能にする
        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            e.Node.BeginEdit();
        }

        // ノードのラベル編集後の処理
        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node == testUsernameNode && e.Label != null)
            {
                string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Properties.Settings.Default.UserName = e.Label + currentTime;
                Properties.Settings.Default.Save();
            }
        }
    }
}

