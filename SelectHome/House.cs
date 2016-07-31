using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SelectHome
{
    class House : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    private int num;
        //asdf
    public int Num
    {
        get
        {
            return num;
        }
        set
        {
            num = value;
            if(this.PropertyChanged!=null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Num"));
            }
        }
    }
}
}
