using System;
using System.Collections.Generic;
using System.IO;

namespace FunGuy
{
    public class CharacterTexture : FunGuy.Texture
    {
        public float Height;
        public const int Down = 0;
        public const int Up = 1;
        public const int Left = 2;
        public const int Right = 3;

        public const int DownTwo = 4;
        public const int DownThree = 5;
        public const int UpTwo = 6;
        public const int UpThree = 7;
        public const int LeftTwo = 8;
        public const int LeftThree = 9;
        public const int RightTwo = 10;
        public const int RightThree = 11;

        public static int NextUp(int currdir)
        {
            if (currdir == Up) {
                return UpTwo; }
            if (currdir == UpTwo) {
                return UpThree; }
           
            return Up;
        }

        public static int NextDown(int currdir)
        {
            if (currdir == Down) {
                return DownTwo; }
            if (currdir == DownTwo) {
                return DownThree; }

            return Down;
        }

        public static int NextLeft(int currdir)
        {
            if (currdir == Left) {
                return LeftTwo; }
            if (currdir == LeftTwo) {
                return LeftThree; }

            return Left;
        }

        public static int NextRight(int currdir)
        {
            if (currdir == Right) {
                return RightTwo; }
            if (currdir == RightTwo) {
                return RightThree; }

            return Right;
        }


        public CharacterTexture(string name, int mapValue, int texLibID, int index, float height)
        {
            Name = name;
            Value = mapValue;
            TexLibID = texLibID;
            Index = index;
            Height = height;
        }
        public CharacterTexture(Texture texture, float height)
        {
            Name = texture.Name;
            Value = texture.Value;
            TexLibID = texture.TexLibID;
            Height = height;
        }

    }
}

