using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;

namespace FunGuy.Input
{

    public class Keyboard : Input
    {
        public Dictionary<int, bool> keys = new Dictionary<int, bool>();

        public Keyboard()
        {
            //save = Keyboard [Key.S];
        }

        int save;
        /*
        public override System.Collections.Generic.Dictionary<string, bool> In()
        {
            Dictionary<string, bool> buttons = new Dictionary<string, bool>();

            if (Keyboard [Key.Down]) {
                buttons.Add("down", true); }
            else {
                buttons.Add("down", false); }

            if (Keyboard [Key.Up]){
                buttons.Add("up", true);}
            else{
                buttons.Add("up", false);}


            if (Keyboard [Key.Right]){
                buttons.Add("right", true);}
            else{
                buttons.Add("right", false);}

            if (Keyboard [Key.Left]){
                buttons.Add("left", true);}
            else{
                buttons.Add("left", false);}

            if ((Keyboard [Key.LControl] || Keyboard [Key.RControl]) && (Keyboard [Key.S]))
            {
                buttons.Add("save", true);}
            else{
                buttons.Add("save", false);}

            if (Keyboard [Key.Comma]){
                buttons.Add("less", true);}
            else{
                buttons.Add("less", false);}

            if (Keyboard [Key.Period]){
                buttons.Add("plus", true);}
            else{
                buttons.Add("plus", false);}
        }

*/
        public override Dictionary<string, bool> In()
        {
            return null;
        } 


    }
}

