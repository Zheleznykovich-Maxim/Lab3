using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;

namespace Lab3
{
    class Philosopher
    {
        private int id;
        private Panel statusPanel;
        private Panel forkStatusPanel1;
        private Panel forkStatusPanel2;
        private PictureBox pictureBox;
        private ListBox actionListBox;
        private int EatTime;
        private int ThinkTime;
        private Mutex leftFork = new Mutex();
        private Mutex rightFork = new Mutex();
        private volatile bool stopPhilosopher = false;
        public int countEat;
        

        public Philosopher(
          int id,
          Mutex leftFork,
          Mutex rightFork,
          ListBox lb,
          int EatTime,
          int ThinkTime)
        {
            this.id = id;
            this.leftFork = leftFork;
            this.rightFork = rightFork;
            this.actionListBox = lb;
            this.EatTime = EatTime;
            this.ThinkTime = ThinkTime;
        }

        public void SetStatusPanel(Panel panel) => this.statusPanel = panel;

        public void SetPictureBox(PictureBox pb) => this.pictureBox = pb;
        public void EatAndThink()
        {
            while (!this.stopPhilosopher)
            {
                countEat++;
                if (countEat >= 4)
                {
                    actionListBox.Invoke(new Action(() =>
                    {
                        actionListBox.Items.Add(new ColoredListBoxItem(
                            $"Философ {id} не ел уже {countEat} циклов!", Color.Red));
                        actionListBox.TopIndex = actionListBox.Items.Count - 1;
                    }));
                }
                this.ChangePhilosopherState("Размышляет");
                Thread.Sleep(ThinkTime);
                this.ChangePhilosopherState("Голоден");
                if (this.leftFork.WaitOne(1000))
                {
                    this.ChangeForkState(true, this.forkStatusPanel1);
                    if (this.rightFork.WaitOne(1000))
                    {
                        this.ChangeForkState(true, this.forkStatusPanel2);
                        this.ChangePhilosopherState("Ест");
                        this.countEat = 0;
                        Thread.Sleep(EatTime);
                        this.rightFork.ReleaseMutex();
                        this.ChangeForkState(false, this.forkStatusPanel2);
                        this.leftFork.ReleaseMutex();
                        this.ChangeForkState(false, this.forkStatusPanel1);
                    }
                    else
                    {
                        this.leftFork.ReleaseMutex();
                        this.ChangeForkState(false, this.forkStatusPanel1);
                    }
                }
            }
        }

        public void SetForkStatusPanel(Panel panel1, Panel panel2)
        {
            this.forkStatusPanel1 = panel1;
            this.forkStatusPanel2 = panel2;
        }
        private void ChangeForkState(bool taken, Panel forkStatusPanel)
        {
            //if (forkStatusPanel == null || !forkStatusPanel.InvokeRequired )
            //{
            //    forkStatusPanel.BackColor = GetColorForForkState(taken);
            //}

            forkStatusPanel.BackColor = GetColorForForkState(taken);
            String action;
            if (taken)
            {
                action = "поднял";
            }
            else
            {
                action = "опустил";
            }
            actionListBox.Invoke(new Action(() =>
            {
                actionListBox.Items.Add(new ColoredListBoxItem(
                    $"Философ {action} вилку {forkStatusPanel.Name}", Color.Black));
                actionListBox.TopIndex = actionListBox.Items.Count - 1;
            }));
                

        }
        private void ChangePhilosopherState(string state)
        {
            //if (this.statusPanel == null || !this.statusPanel.InvokeRequired)
            //    return;
            statusPanel.BackColor = GetColorForState(state);
            actionListBox.Invoke(new Action(() =>
            { 
                actionListBox.Items.Add(new ColoredListBoxItem(
                    $"Философ {id} {state}", Color.Black));
                actionListBox.TopIndex = actionListBox.Items.Count - 1;
            }));
        }
        private Color GetColorForState(string state)
        {
            switch (state)
            {
                case "Размышляет":
                    return Color.Red;
                case "Голоден":
                    return Color.Blue;
                case "Ест":
                    return Color.Green;
                default:
                    return Color.Transparent;
            }
        }
        private Color GetColorForForkState(bool taken) => taken ? Color.Gray : Color.White;
        public void Stop() => this.stopPhilosopher = true;
    }
}
