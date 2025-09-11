using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Buckshot_Roulette
{
    internal class Program
    {
        //아이템 배열 수갑, 맥주, 돋보기, 담배, 톱, 대포폰, 언버터, 아드레날린
        enum item { Handcuffs, Beer, Magnifying_Glass, Cigarette_Pack, Hand_Saw, Burner_Phone, Inverter, Adrenaline }

        //재장전 함수
        static void Reload (int count, ref string[] bullet)
        {
            //랜덤 명령어
            Random rand = new Random();
            //탄의 실/공포 탄 여부
            Boolean Live_Round = false;
            int Random_Count;
            for (int i = 0; i < count; i++) 
            {
                //랜덤으로 1,2 를 받아 1이면 실탄, 2이면 공포탄을 배열에 삽입
                Random_Count = rand.Next(2);
                if (Random_Count == 1)
                {
                    Live_Round = true;
                }
                else 
                {
                    Live_Round = false;
                }

                if (Live_Round == true)
                {
                    bullet[i] = "Live_Round";
                }
                else 
                {
                    bullet[i] = "Blank_round";
                }
            }
        }

        //딜러가 총에 맞았을 때 작동하는 함수
        static void Dealer_bullet_Check(string[] bullet, ref int Bullet_Count, ref int Dealer_HP, int DMG, ref int User_Handcuff, ref int Dealer_Handcuff, ref Boolean User_turn, ref Boolean Dealer_turn)
        {
            //공포탄, 실탄 여부를 확인하고 실탄일 경우 DMG 만큼 체력을 빼고 턴을 넘깁니다.
            if (bullet[Bullet_Count] == "Live_Round")
            {
                if (User_turn == true)
                {
                    if (Dealer_Handcuff == 1)
                    {
                        User_turn = true;
                        Dealer_turn = false;
                        Dealer_Handcuff = 0;
                    }
                    else 
                    {
                        User_turn = false;
                        Dealer_turn = true;
                    }
                }
                else
                {
                    if (User_Handcuff == 1)
                    {
                        User_turn = false;
                        Dealer_turn = true;
                        User_Handcuff = 0;
                    }
                    else
                    {
                        User_turn = true;
                        Dealer_turn = false;
                    }
                }
                Dealer_HP -= DMG;
                if (DMG == 2)
                {
                    DMG = 1;
                }
                Console.WriteLine("탕---!");
                Thread.Sleep(2000);
            }
            else 
            {
                //공포탄을 자신에게 쏜경우 턴을 이어나가고 상대방에게 공포탄을 쏜경우 턴을 넘깁니다.
                if (User_turn == true)
                {
                    if (Dealer_Handcuff == 1)
                    {
                        User_turn = true;
                        Dealer_turn = false;
                        Dealer_Handcuff = 0;
                    }
                    else
                    {
                        User_turn = false;
                        Dealer_turn = true;
                    }                    
                }
                if(Dealer_turn == true)
                {
                    User_turn = false;
                    Dealer_turn = true;
                }
                Console.WriteLine("틱...");
                Thread.Sleep(2000);
            }
            Bullet_Count++;
        }

        //유저가 총에 맞았을 때 작동하는 함수
        static void User_bullet_Check(string[] bullet, ref int Bullet_Count, ref int User_HP, int DMG, ref int User_Handcuff, ref int Dealer_Handcuff, ref Boolean User_turn, ref Boolean Dealer_turn)
        {
            //공포탄, 실탄 여부를 확인하고 실탄일 경우 DMG 만큼 체력을 빼고 턴을 넘깁니다.
            if (bullet[Bullet_Count] == "Live_Round")
            {
                if (User_turn == true)
                {
                    if (Dealer_Handcuff == 1)
                    {
                        User_turn = true;
                        Dealer_turn = false;
                        Dealer_Handcuff = 0;
                    }
                    else 
                    {
                        User_turn = false;
                        Dealer_turn = true;
                    }                    
                }
                else 
                {
                    if (User_Handcuff == 1)
                    {
                        User_turn = false;
                        Dealer_turn = true;
                        User_Handcuff = 0;
                    }
                    else 
                    {
                        User_turn = true;
                        Dealer_turn = false;
                    }
                }
                User_HP -= DMG;
                if (DMG == 2 ) 
                {
                    DMG = 1;
                }
                Console.WriteLine("탕---!");
                Thread.Sleep(2000);
            }
            else
            {
                //공포탄을 자신에게 쏜경우 턴을 이어나가고 상대방에게 공포탄을 쏜경우 턴을 넘깁니다.
                if (User_turn == true)
                {
                    User_turn = true;
                    Dealer_turn = false;
                }
                if(Dealer_turn == true)
                {
                    if (User_Handcuff == 1)
                    {
                        Dealer_turn = true;
                        User_turn = false;
                        User_Handcuff = 0;
                    }
                    else 
                    {
                        User_turn = true;
                        Dealer_turn = false;
                    } 
                }
                Console.WriteLine("틱...");
                Thread.Sleep(2000);
            }   
            Bullet_Count++;
        }

        //새로운 아이템을 받는 함수. 매턴마다 두개의 아이템을 새로 획득합니다.
        static void NewItem(ref string[] User_Items, ref string[] Dealer_Items, ref int User_Item_Count, ref int Dealer_Item_Count) 
        {
            Random rand = new Random();
            for (int i = User_Item_Count; i < User_Item_Count + 2; i++) 
            {
                User_Items[i] = ((item)rand.Next(0, 8)).ToString();
            }
            for (int i = Dealer_Item_Count; i < Dealer_Item_Count + 2; i++) 
            {
                Dealer_Items[i] = ((item)rand.Next(0, 8)).ToString();
            }
            User_Item_Count += 2;
            Dealer_Item_Count += 2;
        }

        //유저가 아이템을 사용할 때 작동하는 함수
        static void UseItem_User(ref string[] User_Items, ref int User_Item_Count, ref string use_item) 
        {
            Console.WriteLine("사용할 아이템을 선택하여 주십시오");
            for (int i = 0; i < User_Item_Count; i++) 
            {
                Console.WriteLine("{0}. {1}",i + 1, User_Items[i]);
            }
            int.TryParse(Console.ReadLine(), out var a);
            use_item = User_Items[a - 1];
            for (int i = a - 1; i < User_Item_Count; i++) 
            {
                var temp = User_Items[i + 1];
                User_Items[i] = temp;
            }
            User_Item_Count--;
        }

        static void UseItem_Dealer(ref string[] Dealer_Items, ref int Dealer_Item_Count, ref string use_item) 
        {
            Random rand = new Random();
            var a = rand.Next(0, Dealer_Item_Count);
            use_item = Dealer_Items[a];
            Console.WriteLine("딜러가 {0}을(를) 사용합니다.", use_item);
            for (int i = a; i < Dealer_Item_Count; i++)
            {
                var temp = Dealer_Items[i + 1];
                Dealer_Items[i] = temp;
            }
            Dealer_Item_Count--;
        }

        // 맥주 아이템 사용시 작동하는 함수입니다. 장전된 핸져 탄을 제거합니다.
        static void Beer(ref string[] bullet, ref int Bullet_Count) 
        {                
            Console.WriteLine("탁-!");
            Thread.Sleep(1000);
            Console.WriteLine("꿀꺽꿀꺽");
            Thread.Sleep(1000);
            Console.WriteLine("철컥-!");
            Thread.Sleep(1000);
            Console.WriteLine("{0}이 한발 빠졌다", bullet[Bullet_Count]);
            Bullet_Count++;
        }

        //수갑 아이템 사용시 작동하는 함수입니다. 대상의 턴을 한 번 스킵합니다.
        static void Handcuffs(ref int Handcuffs) 
        {
            if (Handcuffs != 1) 
            {
                Console.WriteLine("찰칵---");
                Thread.Sleep(2000);
                Handcuffs = 1;
            }
        }

        //돋보기 아이템 사용시 작동하는 함수입니다. 현재 장전된 탄을 확인합니다.
        static void Magnifying_Glass(ref string[] bullet, int Bullet_Count) 
        {
            Console.WriteLine("쨍그랑-!");
            Thread.Sleep(1000);
            Console.WriteLine("현재 탄환은 {0} 이다.", bullet[Bullet_Count]);
            Thread.Sleep(2000);
        }

        //담배 아이템 사용시 작동하는 함수입니다. 현재 체력을 1회복합니다. 최대체력 5이상 회복하지 않습니다.
        static void Cigarette_Pack(ref int HP)
        {
            Console.WriteLine("치익--");
            Thread.Sleep(1000);
            Console.WriteLine("후우--");
            Thread.Sleep(2000);
            if (HP != 5)
            {
                HP += 1;
            }
        }

        //톱 아이템 사용시 작동하는 함수입니다. DMG를 2로 늘립니다.
        static void Hand_Saw(ref int DMG) 
        {
            if (DMG != 2)
            {
                Console.WriteLine("슥삭슥삭-!");
                Thread.Sleep(1000);
                Console.WriteLine("챙그랑-!");
                Thread.Sleep(1000);
                DMG = 2;
            }
        }

        //대포폰 아이템 사용시 작동하는 함수입니다. 남은탄이 2발이상일 경우 무작위 남은탄 하나의 정보를 알려줍니다.
        static void Burner_Phone(ref string[] bullet, ref int Bullet_Count, ref int Bullet_limit) 
        {
            if (Bullet_limit - Bullet_Count >= 2)
            {
                Random rand = new Random();
                int Phone = rand.Next(Bullet_Count, Bullet_limit);
                Console.WriteLine("착-!");
                Thread.Sleep(1000);
                if (bullet[Phone] == "Live_Round")
                {
                    Console.WriteLine("{0}번째 탄은... 실탄이다.", Phone - Bullet_Count + 1);
                }
                else 
                {
                    Console.WriteLine("{0}번째 탄은... 공포탄이다.", Phone - Bullet_Count + 1);
                }
                Thread.Sleep(1000);
            }
            else 
            {
                Console.WriteLine("착-!");
                Thread.Sleep(1000);
                Console.WriteLine("안타깝게... 됐군...");
                Thread.Sleep(1000);
            }
        }

        //인버터 아이템 사용시 작동하는 함수입니다. 현재 장전된 탄을 다른 탄으로 변경합니다.
        static void Inverter(ref string[] bullet, ref int Bullet_Count) 
        {
            Console.WriteLine("삐빅-!");
            Thread.Sleep(1000);
            Console.WriteLine("찰칵-!");
            Thread.Sleep(1000);
            if (bullet[Bullet_Count] == "Live_Round") 
            {
                bullet[Bullet_Count] = "Blank_round";
            }
            if (bullet[Bullet_Count] == "Blank_round") 
            {
                bullet[Bullet_Count] = "Live_Round";
            }
        }

        //유저의 아드레날린 사용시 작동하는 함수입니다. 딜러의 아이템을 하나 선택하여 가져옵니다.
        static void Adrenaline(ref string[] Dealer_Items, ref int Dealer_Item_Count, ref string use_item, ref string[] User_Items, ref int User_Item_Count)
        {
            Console.WriteLine("빼앗아 올 아이템을 선택하여 주십시오");
            for (int i = 0; i < Dealer_Item_Count; i++)
            {
                Console.WriteLine("{0}. {1}", i + 1, Dealer_Items[i]);
            }
            int.TryParse(Console.ReadLine(), out var a);
            use_item = Dealer_Items[a - 1];
            for (int i = a - 1; i < Dealer_Item_Count; i++)
            {
                var temp = Dealer_Items[i + 1];
                Dealer_Items[i] = temp;
            }
            Dealer_Item_Count--;
            User_Items[User_Item_Count] = use_item;
            User_Item_Count++;
        }

        static void Adrenaline_Dealer(ref string[] Dealer_Items, ref int Dealer_Item_Count, ref string use_item, ref string[] User_Items, ref int User_Item_Count)
        {
            Random rand = new Random();
            var a = rand.Next(0, User_Item_Count);
            use_item = User_Items[a];
            Console.WriteLine("{0} 아이템을 가져갑니다", use_item);
            for (int i = a; i < User_Item_Count; i++)
            {
                var temp = User_Items[i + 1];
                User_Items[i] = temp;
            }
            User_Item_Count--;
            Dealer_Items[Dealer_Item_Count] = use_item;
            Dealer_Item_Count++;
        }

        static void Main(string[] args)
        {
            string[] bullet = new string[8];
            Random rand = new Random();
            int Live_Round_Count = 0;
            int Blank_Round_Count = 0;
            int Bullet_Count = 0;
            int Bullet_limit = 0;
            int User_HP = 5;
            int Dealer_HP = 5;
            int User_Item_Count = 0;
            int Dealer_Item_Count = 0;
            int Dealer_shot;
            int User_Handcuff = 0;
            int Dealer_Handcuff = 0;
            int DMG = 1;
            string use_item = null;
            string[] User_Items = new string[10];
            string[] Dealer_Items = new string[10];
            Boolean User_turn = true;
            Boolean Dealer_turn = false;
            ConsoleKeyInfo keyInfo;
            Bullet_limit = rand.Next(4, 8);
            Reload(Bullet_limit, ref bullet);
            NewItem(ref User_Items, ref Dealer_Items, ref User_Item_Count, ref Dealer_Item_Count);
            while (User_HP > 0 && Dealer_HP > 0) 
            {
                Console.Clear();
                if (Bullet_Count == Bullet_limit)
                {
                    Console.WriteLine("딜러가 탄환을 재장전합니다.");
                    Thread.Sleep(2000);
                    Console.Clear();
                    Bullet_limit = rand.Next(4, 8);
                    Reload(Bullet_limit, ref bullet);
                    NewItem(ref User_Items, ref Dealer_Items, ref User_Item_Count, ref Dealer_Item_Count);
                    Bullet_Count = 0;
                }
                Live_Round_Count = 0;
                Blank_Round_Count = 0;

                for (int i = Bullet_Count; i < Bullet_limit; i++)
                {
                    if (bullet[i] == "Live_Round")
                    {
                        Live_Round_Count += 1;
                    }

                    if (bullet[i] == "Blank_round")
                    {
                        Blank_Round_Count += 1;
                    }
                }

                Console.WriteLine("공포탄 {0}발", Blank_Round_Count);
                Console.WriteLine("실탄 {0}발", Live_Round_Count);
                Console.WriteLine("총합 {0}발", Blank_Round_Count + Live_Round_Count);
                Console.WriteLine("현재 딜러의 체력: {0}", Dealer_HP);
                Console.WriteLine("현재 당신의 체력: {0}", User_HP);
                Console.Write("현재 당신의 아이템: ");
                for (int i = 0; i < User_Item_Count; i++)
                {
                    Console.Write("{0} ", User_Items[i]);
                }
                Console.WriteLine("");
                Console.Write("현재 딜러의 아이템: ");
                for (int i = 0; i < Dealer_Item_Count; i++)
                {
                    Console.Write("{0} ", Dealer_Items[i]);
                }
                Console.WriteLine("");

                if (User_turn == true && User_Handcuff != 1)
                {
                    Console.WriteLine("1.딜러에게 사격");
                    Console.WriteLine("2.나에게 사격");
                    Console.WriteLine("3.아이템 사용");
                    keyInfo = Console.ReadKey(true);

                    if (keyInfo.Key == ConsoleKey.D1)
                    {
                        Console.WriteLine("딜러에게 사격합니다.");
                        Dealer_bullet_Check(bullet, ref Bullet_Count, ref Dealer_HP, DMG, ref User_Handcuff, ref Dealer_Handcuff, ref User_turn, ref Dealer_turn);
                    }

                    if (keyInfo.Key == ConsoleKey.D2)
                    {
                        Console.WriteLine("총구를 자신에게 향합니다.");
                        User_bullet_Check(bullet, ref Bullet_Count, ref User_HP, DMG, ref User_Handcuff, ref Dealer_Handcuff, ref User_turn, ref Dealer_turn);
                    }

                    if (keyInfo.Key == ConsoleKey.D3)
                    {
                        Console.Clear();
                        UseItem_User(ref User_Items, ref User_Item_Count, ref use_item);
                        if (use_item == "Beer") 
                        {
                            Beer(ref bullet, ref Bullet_Count);
                        }

                        if (use_item == "Handcuffs") 
                        {
                            Handcuffs(ref Dealer_Handcuff);
                        }

                        if (use_item == "Magnifying_Glass") 
                        {
                            Magnifying_Glass(ref bullet, Bullet_Count);
                        }

                        if (use_item == "Cigarette_Pack") 
                        {
                            Cigarette_Pack(ref User_HP);
                        }

                        if (use_item == "Hand_Saw") 
                        {
                            Hand_Saw(ref DMG);
                        }

                        if (use_item == "Burner_Phone") 
                        {
                            Burner_Phone(ref bullet, ref Bullet_Count, ref Bullet_limit);
                        }

                        if (use_item == "Inverter") 
                        {
                            Inverter(ref bullet, ref Bullet_Count);
                        }

                        if (use_item == "Adrenaline") 
                        {
                            Adrenaline(ref Dealer_Items, ref Dealer_Item_Count, ref use_item, ref User_Items, ref User_Item_Count);
                        }
                    }
                }

                else if (Dealer_turn == true && Dealer_Handcuff != 1)
                {
                    Console.Clear();
                    Console.WriteLine("딜러: 내 차례군");
                    if (Dealer_Item_Count != 0)
                    {
                        Console.Clear();
                        UseItem_Dealer(ref Dealer_Items, ref Dealer_Item_Count, ref use_item);
                        if (use_item == "Beer")
                        {
                            Beer(ref bullet, ref Bullet_Count);
                        }

                        if (use_item == "Handcuffs")
                        {
                            Handcuffs(ref User_Handcuff);
                        }

                        if (use_item == "Magnifying_Glass")
                        {
                            Magnifying_Glass(ref bullet, Bullet_Count);
                        }

                        if (use_item == "Cigarette_Pack")
                        {
                            Cigarette_Pack(ref Dealer_HP);
                        }

                        if (use_item == "Hand_Saw")
                        {
                            Hand_Saw(ref DMG);
                        }

                        if (use_item == "Burner_Phone")
                        {
                            Burner_Phone(ref bullet, ref Bullet_Count, ref Bullet_limit);
                        }

                        if (use_item == "Inverter")
                        {
                            Inverter(ref bullet, ref Bullet_Count);
                        }

                        if (use_item == "Adrenaline")
                        {
                            Adrenaline_Dealer(ref Dealer_Items, ref Dealer_Item_Count, ref use_item, ref User_Items, ref User_Item_Count);
                        }
                    }

                    if (Bullet_Count + 1 == Bullet_limit || Blank_Round_Count == 0 || Live_Round_Count == 0 || use_item == "Magnifying_Glass")
                    {
                        if (bullet[Bullet_Count] == "Live_Round")
                        {
                            Console.WriteLine("딜러: 정해진 결과로군");
                            Thread.Sleep(2000);
                            Console.WriteLine("딜러가 당신을 향해 총구를 겨눕니다.");
                            Thread.Sleep(2000);
                            User_bullet_Check(bullet, ref Bullet_Count, ref User_HP, DMG, ref User_Handcuff, ref Dealer_Handcuff, ref User_turn, ref Dealer_turn);
                        }
                        else if (bullet[Bullet_Count] == "Blank_round")
                        {
                            Console.WriteLine("딜러: 정해진 결과로군");
                            Thread.Sleep(2000);
                            Console.WriteLine("딜러가 스스로를 향해 총구를 겨눕니다.");
                            Thread.Sleep(2000);
                            Dealer_bullet_Check(bullet, ref Bullet_Count, ref Dealer_HP, DMG, ref User_Handcuff, ref Dealer_Handcuff, ref User_turn, ref Dealer_turn);
                        }
                    }

                    else
                    {
                        Dealer_shot = rand.Next(2);
                        if (Dealer_shot == 1)
                        {
                            Console.WriteLine("딜러가 총구를 당신에게 겨눕니다.");
                            User_bullet_Check(bullet, ref Bullet_Count, ref User_HP, DMG, ref User_Handcuff, ref Dealer_Handcuff, ref User_turn, ref Dealer_turn);
                        }
                        else
                        {
                            Console.WriteLine("딜러가 스스로에게 총구를 겨눕니다.");
                            Dealer_bullet_Check(bullet, ref Bullet_Count, ref Dealer_HP, DMG, ref User_Handcuff, ref Dealer_Handcuff, ref User_turn, ref Dealer_turn);
                        }
                    }
                }
            }

            if (User_HP < 0)
            {
                Console.WriteLine("패배하였습니다.");
            }
            else 
            {
                Console.WriteLine("승리하였습니다.");
            }
        }
    }
}
