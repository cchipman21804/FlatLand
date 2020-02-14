using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flatland
{
    class Program
    {
        static void Main(string[] args)
        {
            /*

         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                                                                       *
         *                         INITIALIZATION                                *
         *                                                                       *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 
             */

            // Specify location & dimensions of 2D Flatland
            const int xoffset = 3;          // Where to display Flatland on screen
            const int yoffset = 4;
            const int maxX = 100;           // How wide is Flatland
            const int maxY = 32;            // How long is Flatland

            // This will be determined by difficulty level.
            const int obstacleDensity = 25; // Specifies how crowded with obstacles
                                            // (lower number is more crowded)

            // Create Flatland
            char[,] flatLand = new char[maxX, maxY];

            // Specify characters in 2D Flatland
            const char obstacle = '\u256c'; // Unicode line drawing character. Originally ASCII '#' or ASCII 'X'
            const char citizen = 'c';
            const char soldier = '$';
            const char human = 'i';
            const char teleport = 'T';
            const char weapon = '!';
            const char firstaid = '+';
            const char armor = 'A';
            const char emptyspace = ' ';

            // Specify limits
            const int minCitizens = 2;
            const int maxCitizens = 8;
            const int citizenDamage = 5;    // Set maximum unarmed citizen's damage to humans
            const int citizenWallet = 100;  // Set maximum unarmed citizen's wallet capacity

            const int minSoldiers = 2;
            const int maxSoldiers = 8;
            const int soldierDamage = 10;   // Set maximum armed soldier's damage to humans
            const int soldierWallet = 50;   // Set maximum armed soldier's wallet capacity

            const int minTeleports = 2;
            const int maxTeleports = 50;

            const int minWeapons = 1;
            const int maxWeapons = 10;

            const int minFirstAid = 1;
            const int maxFirstAid = 10;

            const int minArmor = 1;
            const int maxArmor = 10;

            Random random = new Random();                   // Set difficulty level at random

            // Keep track of multiple inanimate objects

            int numTeleports = random.Next(minTeleports,maxTeleports);

            int numWeapons = minWeapons;                    // This is not a consumable item yet.
                                                            // Weapons are persistent.
            int[] weaponx = new int[numWeapons];            // Save the locations so they will not be
            int[] weapony = new int[numWeapons];            // disturbed by non-human characters.

            int numFirstAid = minFirstAid;                  // This is not a consumable item yet.
                                                            // Bandages are persistent.
            int[] firstaidx = new int[numFirstAid];         // Save the locations so they will not be
            int[] firstaidy = new int[numFirstAid];         // disturbed by non-human characters.

            int numArmor = minArmor;                        // This is not a consumable item yet.
                                                            // Armor is persistent.
            int[] armorx = new int[numArmor];               // Save the locations so they will not be
            int[] armory = new int[numArmor];               // disturbed by non-human characters.

            // Keep track of multiple animate objects
            int numCitizens = random.Next(minCitizens,maxCitizens);
            int[] citizenx = new int[numCitizens];          // Tracks current x,y-coordinates
            int[] citizeny = new int[numCitizens];
            int[] citizenx1 = new int[numCitizens];         // Tracks previous x,y-coordinates
            int[] citizeny1 = new int[numCitizens];
            int[] citizenhp = new int[numCitizens];         // Tracks health
            int[] citizenGold = new int[numCitizens];       // Tracks wallet

            int numSoldiers = random.Next(minSoldiers,maxSoldiers);
            int[] soldierx = new int[numSoldiers];          // Tracks current x,y-coordinates
            int[] soldiery = new int[numSoldiers];
            int[] soldierx1 = new int[numSoldiers];         // Tracks previous x,y-coordinates
            int[] soldiery1 = new int[numSoldiers];
            int[] soldierhp = new int[numSoldiers];         // Tracks health
            int[] soldierGold = new int[numSoldiers];       // Tracks wallet

            int totalEnemyChars = numCitizens + numSoldiers;
            int damage;

            int humanx;
            int humany;
            int humanx1;
            int humany1;
            int humanhp;                            // Set human's health (0% = dead, 100% = undamaged)
            int humanGold;                          // Set human's wallet
            const int humanDamage = 5;              // Set maximum unarmed human's attack damage to others
            const int weaponDamage = 10;            // Set maximum armed human's attack damage to others
            string weaponStatus = "";               // Give the player this many weapons
            int ammo = weaponStatus.Length;         // This many weapons are in the human's inventory
            bool isArmed = false;
            string armorStatus = "";                // Give the player this much armor
            int shield = armorStatus.Length;        // The human has this much armor in inventory
            string firstAidStatus = "";             // Give the player this many first aid kits
            int bandages = firstAidStatus.Length;   // This many first aid kits are in the human's inventory

            int x;                                  // Generic x-coordinate
            int y;                                  // Generic y-coordinate
            int deltax;                             // Calculated change in x
            int deltay;                             // Calculated change in y

            int n;                                  // Generic loop counter
            string userPrompt;                      // Generic user input


            const string title = "Flat Land";
            const string version = "v1.0";
            const string cmdNotRecognized = "That command is not recognized.  Please try again.";
            const string closingMessage = "Thanks for playing!  Press [ENTER] to close...";

            // Building East and West walls on the boundaries of Flatland.
            for (y = 0; y < maxY; y++)
            {
                flatLand[0, y] = obstacle;
                flatLand[maxX - 1, y] = obstacle;
            }

            // Building North and South walls on the boundaries of Flatland.
            for (x = 0; x < maxX; x++)
            {
                flatLand[x, 0] = obstacle;
                flatLand[x, maxY - 1] = obstacle;
            }

            /*
         * 
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                                                                       *
         *               N E W    G A M E   S T A R T S   H E R E                *
         *                                                                       *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *
            */
         Initialize:

            // Reset endOfGame flag
            bool endOfGame = false;

            // Placing obstacles at random locations within Flatland &
            // fill the rest with empty space.

            for (y = 1; y < maxY - 1; y++)
            {
                for (x = 1; x < maxX - 1; x++)
                {
                    n = random.Next(1, obstacleDensity + 1);    // Change upper limit to change number of obstacles
                    if (n == obstacleDensity / 2)               // Keep this number about halfway between the limits
                    {
                        flatLand[x, y] = obstacle;
                    }
                    else
                    {
                        flatLand[x, y] = emptyspace;
                    }
                }
            }

            // Place teleports at random in Flatland.
            for (n = 0; n < numTeleports; n++)
            {
                TeleportCharacter();
                flatLand[x, y] = teleport;
            }

            // Place weapons at random in Flatland.
            for (n = 0; n < numWeapons; n++)
            {
                TeleportCharacter();
                flatLand[x, y] = weapon;
                weaponx[n] = x;
                weapony[n] = y;
            }

            // Place first aid kits at random in Flatland.
            for (n = 0; n < numFirstAid; n++)
            {
                TeleportCharacter();
                flatLand[x, y] = firstaid;
                firstaidx[n] = x;
                firstaidy[n] = y;
            }

            // Place armor at random in Flatland.
            for (n = 0; n < numArmor; n++)
            {
                TeleportCharacter();
                flatLand[x, y] = armor;
                armorx[n] = x;
                armory[n] = y;
            }

            // Place citizens at random in Flatland.
            for (n = 0; n < numCitizens; n++)
            {
                TeleportCharacter();
                citizenx[n] = x;
                citizeny[n] = y;
                citizenx1[n] = citizenx[n];
                citizeny1[n] = citizeny[n];
                citizenhp[n] = 100;
                citizenGold[n] = random.Next(1, citizenWallet);
                flatLand[citizenx[n], citizeny[n]] = citizen;
            }

            // Place soldiers at random in Flatland.
            for (n = 0; n < numSoldiers; n++)
            {
                TeleportCharacter();
                soldierx[n] = x;
                soldiery[n] = y;
                soldierx1[n] = soldierx[n];
                soldiery1[n] = soldiery[n];
                soldierhp[n] = 100;
                soldierGold[n] = random.Next(1, soldierWallet);
                flatLand[soldierx[n], soldiery[n]] = soldier;
            }

            // Place player at random in Flatland.
            TeleportCharacter();
            humanx = x;
            humany = y;
            humanx1 = humanx;
            humany1 = humany;
            humanhp = 100;
            humanGold = 0;
            flatLand[x, y] = human;

            Console.Clear();
            DisplayLabels();

            while (!endOfGame)                  // Play the game
            {
                EnemyStatus();
                HumanStatus();
                DisplayFlatLand();
                HumanMove();
                CitizensMove();
                SoldiersMove();

                /*
                 * Character behaviors - [$]oldiers vs [c]itizens
                 * 
                 * Citizens are docile, mostly wandering at random. They will retreat from the human
                 * if the human gets too close (define 'too close'). They will attack if backed into
                 * a corner by a human.
                 * 
                 * Soldiers remain stationary until an armed human passes within five cells of their
                 * position. At that point, a soldier will aggressively seek out the human and attack.
                 * 
                 * 
                 */


            }
            // Place the closing message on the screen at these coordinates:
            x = xoffset;
            y = maxY + yoffset + 5; // Any further down than +5 scrolls the message off the screen
            Console.SetCursorPosition(x, y);
            Console.Write($"{closingMessage}");
            Console.ReadLine();

            // Return to Operating System

            /*
             * 
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
             *                                                                       *
             *                         S U B R O U T I N E S                         *
             *                                                                       *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
             *
            */

            void DisplayLabels()
            {
                // Display title & version on the screen at these coordinates:
                x = 45;
                y = 0;
                Console.SetCursorPosition(x, y);
                Console.Write($"{title} {version}");

                y += 2;

                // Display column headers
                while (y < yoffset - 1)
                {
                    for (x = 0; x < 100; x += 10)
                    {
                        Console.SetCursorPosition(x + xoffset, y);
                        Console.Write(x / 10);
                        if (x > 9)
                        {
                            Console.SetCursorPosition(x + xoffset + 1, y);
                            Console.Write(0);
                        }
                    }

                    y++;
                    x = 0;

                    while (x < maxX)
                    {
                        for (n = 0; n < 10; n++)
                        {
                            Console.SetCursorPosition(x + xoffset, y);
                            Console.Write(n);
                            x++;
                        }
                    }
                }

                x = 0;

                // Display row numbers
                for (n = 0; n < maxY; n++)
                {
                    Console.SetCursorPosition(x, n + yoffset);
                    if (n < 10)
                    {
                        Console.Write($"0{n}: ");
                    }
                    else
                    {
                        Console.Write($"{n}: ");
                    }
                }
                return;
            }

            void EnemyStatus()    // Display enemy status
            {           
                x = maxX + xoffset + 1;
                y = yoffset - 2;

                Console.SetCursorPosition(x, y);
                Console.Write("Enemy Status:");

                y++;

                while (y < totalEnemyChars)
                {
                    for (n = 0; n < numCitizens; n++)
                    {
                        // Set text color based on HP level
                        Console.SetCursorPosition(x, y);
                        Console.Write($"{citizen} #{n}: @({citizenx[n]},{citizeny[n]})  ");
                        y++;
                        Console.SetCursorPosition(x, y);
                        Console.Write($"\t{citizenhp[n]} HP ");
                        y++;
                    }

                    for (n = 0; n < numSoldiers; n++)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write($"{soldier} #{n}: @({soldierx[n]},{soldiery[n]})  ");
                        y++;
                        Console.SetCursorPosition(x, y);
                        Console.Write($"\t{soldierhp[n]} HP  ");
                        y++;
                    }
                    y++;
                }
                return;
            }

            void HumanStatus()    // Display human's status
            {
                x = xoffset;
                y = yoffset + maxY;

                // Set text color based on HP level
                Console.SetCursorPosition(x, y);
                Console.Write("Player Status:");
                y++;
                Console.SetCursorPosition(x, y);
                Console.Write($"@({humanx},{humany}) - {humanhp} HP".PadRight(20));
                y++;
                Console.SetCursorPosition(x, y);
                Console.Write($"Weapons:  [{weaponStatus}]");
                y++;
                Console.SetCursorPosition(x, y);
                Console.Write($"Armor:    [{armorStatus}]");
                y++;
                Console.SetCursorPosition(x, y);
                Console.Write($"First Aid:[{firstAidStatus}]");
                x = xoffset + 30;
                y = yoffset + maxY;
                Console.SetCursorPosition(x, y);
                Console.Write("Moves:\t      NW     NORTH     NE");
                y++;
                Console.SetCursorPosition(x, y);
                Console.Write("\t        [T/7][Y/8][U/9]\t\t[+/=]: Physician, heal thyself!"); // with weapon");
                y++;
                Console.SetCursorPosition(x, y);
                Console.Write("\t   WEST [G/4][*/5][H/6] EAST\t[ Q ]: Quit / Surrender");
                y++;
                Console.SetCursorPosition(x, y);
                Console.Write($"\t        [V/1][B/2][N/3]\t\tGold: {humanGold}");
                y++;
                Console.SetCursorPosition(x, y);
                Console.Write("\t      SW     SOUTH     SE");

                //y++;
                //Console.SetCursorPosition(x, y);
                //Console.WriteLine();
                return;
            }

            void DisplayFlatLand()
            {
                // Display contents of Flat Land
                for (y = 0; y < maxY; y++)
                {
                    for (x = 0; x < maxX; x++)
                    {
                        Console.SetCursorPosition(x + xoffset, y + yoffset);
                        Console.Write(flatLand[x, y]);
                    }
                }
                return;
            }

            void TeleportCharacter()
            {
                do
                {
                    x = random.Next(1, maxX);
                    y = random.Next(1, maxY);
                }
                while ((flatLand[x, y] != emptyspace));
                return;
            }

            void HumanMove()
            {
                GetAnotherKey:
                humanx1 = humanx;
                humany1 = humany;

                x = xoffset;
                y = maxY + yoffset + 5; // Any further down than +5 scrolls the message off the screen
                Console.SetCursorPosition(x, y);
                Console.Write(" ".PadRight(70));
                Console.SetCursorPosition(x, y);
                Console.Write("Your move: ");
                
                // Wait for player to make a move
                userPrompt = Console.ReadLine();

                // Evaluate player's move
                // Player has the option to use letters or numeric keypad to navigate
                // the playing field
                if (userPrompt == "T" || userPrompt == "t" || userPrompt == "7")
                    {
                        // Move NW
                        humanx--;
                        humany--;
                        x = xoffset + 65;
                        y = maxY + yoffset + 5; // Any further down than +5 scrolls the message off the screen
                        Console.SetCursorPosition(x, y);
                        Console.Write(" ".PadRight(cmdNotRecognized.Length));
                    }
                else if (userPrompt == "Y" || userPrompt == "y" || userPrompt == "8")
                    {
                        // Move N
                        humany--;
                        x = xoffset + 65;
                        y = maxY + yoffset + 5; // Any further down than +5 scrolls the message off the screen
                        Console.SetCursorPosition(x, y);
                        Console.Write(" ".PadRight(cmdNotRecognized.Length));
                    }
                else if (userPrompt == "U" || userPrompt == "u" || userPrompt == "9")
                    {
                        // Move NE
                        humanx++;
                        humany--;
                        x = xoffset + 65;
                        y = maxY + yoffset + 5; // Any further down than +5 scrolls the message off the screen
                        Console.SetCursorPosition(x, y);
                        Console.Write(" ".PadRight(cmdNotRecognized.Length));
                    }
                else if (userPrompt == "G" || userPrompt == "g" || userPrompt == "4")
                    {
                        // Move W
                        humanx--;
                        x = xoffset + 65;
                        y = maxY + yoffset + 5; // Any further down than +5 scrolls the message off the screen
                        Console.SetCursorPosition(x, y);
                        Console.Write(" ".PadRight(cmdNotRecognized.Length));
                    }
                else if (userPrompt == "H" || userPrompt == "h" || userPrompt == "6")
                    {
                        // Move E
                        humanx++;
                        x = xoffset + 65;
                        y = maxY + yoffset + 5; // Any further down than +5 scrolls the message off the screen
                        Console.SetCursorPosition(x, y);
                        Console.Write(" ".PadRight(cmdNotRecognized.Length));
                    }
                else if (userPrompt == "V" || userPrompt == "v" || userPrompt == "1")
                    {
                        // Move SW
                        humanx--;
                        humany++;
                        x = xoffset + 65;
                        y = maxY + yoffset + 5; // Any further down than +5 scrolls the message off the screen
                        Console.SetCursorPosition(x, y);
                        Console.Write(" ".PadRight(cmdNotRecognized.Length));
                    }
                else if (userPrompt == "B" || userPrompt == "b" || userPrompt == "2")
                    {
                        // Move S
                        humany++;
                        x = xoffset + 65;
                        y = maxY + yoffset + 5; // Any further down than +5 scrolls the message off the screen
                        Console.SetCursorPosition(x, y);
                        Console.Write(" ".PadRight(cmdNotRecognized.Length));
                    }
                else if (userPrompt == "N" || userPrompt == "n" || userPrompt == "3")
                    {
                        // Move SE
                        humanx++;
                        humany++;
                        x = xoffset + 65;
                        y = maxY + yoffset + 5; // Any further down than +5 scrolls the message off the screen
                        Console.SetCursorPosition(x, y);
                        Console.Write(" ".PadRight(cmdNotRecognized.Length));
                    }
                else if (userPrompt == "*" || userPrompt == "5")
                    {
                        // Don't move
                        goto DontMove;
                        // x = xoffset;
                        // y = maxY + yoffset + 5; // Any further down than +5 scrolls the message off the screen
                        // Console.SetCursorPosition(x, y);
                        // Console.Write(" ".PadRight(50));
                    }
                else if(userPrompt == "Q" || userPrompt == "q")
                    {
                    endOfGame = true;
                    }

                else if (userPrompt == "+" || userPrompt == "=")
                    {
                        // Does the human have any first aid kits in inventory?
                        if (bandages > 0)
                        {
                            // First aid kits are persistent at this point
                            humanhp += 10;                          // At 10 HP per use
                            if (humanhp > 100) { humanhp = 100; }   // Human health cannot exceed 100%
                        }
                    }

                // Add additional commands here as 'else if (...) {}

                else
                {
                    x = xoffset + 65;
                        y = maxY + yoffset + 5; // Any further down than +5 scrolls the message off the screen
                        Console.SetCursorPosition(x, y);
                        Console.Write($"{cmdNotRecognized}");
                        goto GetAnotherKey;
                    }

                DontMove:

                    // Is the player's requested position occupied by a wall or a robot?
                    if (flatLand[humanx, humany] == obstacle) 
                    {
                        // Don't allow.
                        // Go back to previous position & wait for player to make another move.
                        //x = xoffset;
                        //y = maxY + yoffset + 5; // Any further down than +5 scrolls the message off the screen
                        //Console.SetCursorPosition(x, y);
                        //Console.Write("Another character occupies that space.");
                        humanx = humanx1;
                        humany = humany1;
                        goto GetAnotherKey;
                    }

                    // Is the player's requested position occupied by a teleport?
                    if (flatLand[humanx, humany] == teleport)
                    {
                        // Get out of that sticky situation!
                        TeleportCharacter();
                        humanx = x;
                        humany = y;
                    }

                    // Is the player's requested position occupied by a weapon?
                    if (flatLand[humanx,humany] == weapon)
                    {
                        // Pick it up
                        weaponStatus += weapon;
                        ammo++;
                        if (ammo > 0)
                        { isArmed = true; }
                        else { isArmed = false; }
                        
                    for (n = 0; n < numWeapons; n++)
                        {
                            if (weaponx[n] == humanx && weapony[n] == humany)
                            {
                                weaponx[n] = 0;   // Remove it from Flatland so a non-human character
                                weapony[n] = 0;   // will not bring it back into existance.
                            }
                        }
                    }
                    
                    // Is the player's requested position occupied by a first aid kit?
                    if (flatLand[humanx,humany] == firstaid)
                    {
                        // Pick it up
                        firstAidStatus += firstaid;
                        bandages++;
                        for (n = 0; n < numFirstAid; n++)
                        {
                            if (firstaidx[n] == humanx && firstaidy[n] == humany)
                                {
                                    firstaidx[n] = 0;   // Remove it from Flatland so a non-human
                                    firstaidy[n] = 0;   // character will not bring it back into existance.
                                }
                        }
                    }

                    // Is the player's requested position occupied by armor?
                    if (flatLand[humanx, humany] == armor)
                    {
                        // Pick it up
                        armorStatus += armor;
                        shield++;
                        for (n = 0; n < numArmor; n++)
                        {
                            if (armorx[n] == humanx && armory[n] == humany)
                            {
                                armorx[n] = 0;   // Remove it from Flatland so a non-human character
                                armory[n] = 0;   // will not bring it back into existance.
                            }
                        }
                    }

                    // Is the player's requested position occupied by a citizen or soldier?
                    if (flatLand[humanx, humany] == citizen)
                    {
                        // Identify the occupant
                        // Attack the occupant
                        // Occupant attacks back if health allows
                        for (n = 0; n < numCitizens; n++)
                        {
                            if (humanx == citizenx[n] && humany == citizeny[n])
                            {
                                // Occupant identified - commence attack
                                if (isArmed)
                                {
                                    citizenhp[n] -= random.Next(1, weaponDamage);
                                    // Remove one weapon from inventory? or is weapon persistent?
                                    
                                    // At this point weapons are persistent
                                }
                                else {citizenhp[n] -= random.Next(1, humanDamage);}

                                if (citizenhp[n] <= 0) // Did the citizen die?
                                {
                                    // Remove citizen from Flatland
                                    citizenhp[n] = 0;
                                    flatLand[citizenx[n], citizeny[n]] = emptyspace;
                                    citizenx[n] = 0;
                                    citizeny[n] = 0;
                                    humanGold += citizenGold[n];    // Human takes the gold
                                    citizenGold[n] = 0;
                                }
                                
                                // Citizen strikes back
                                if (citizenhp[n] > 0) 
                                {
                                    // Is the human carrying a shield?

                                    // At this point armor is persistent
                                    if (shield == 0)
                                    {
                                        damage = random.Next(1, citizenDamage);
                                        humanhp -= damage;
                                    }
                                    else
                                    {
                                        damage = random.Next(1, citizenDamage);
                                        humanhp -= damage/2;
                                    }
                                    
                                    if (humanhp < 0)
                                    {
                                        humanhp = 0;
                                        endOfGame = true;
                                        break;
                                    }
                                }
                                
                            }
                        }
                        humanx = humanx1;
                        humany = humany1;
                    }

                    if (flatLand[humanx, humany] == soldier)
                    {
                        for (n = 0; n < numSoldiers; n++)
                        {
                            if (humanx == soldierx[n] && humany == soldiery[n])
                            {
                                // Occupant identified - commence attack
                                if (isArmed)
                                {
                                    soldierhp[n] -= random.Next(1, weaponDamage);
                                    // Remove one weapon from inventory

                                    // At this point weapons are persistent
                                }
                                else { soldierhp[n] -= random.Next(1, humanDamage); }

                                if (soldierhp[n] <= 0)
                                {
                                    // Remove soldier from Flatland
                                    soldierhp[n] = 0;
                                    flatLand[soldierx[n], soldiery[n]] = emptyspace;
                                    soldierx[n] = 0;
                                    soldiery[n] = 0;
                                    humanGold += soldierGold[n];    // Human takes the gold
                                    soldierGold[n] = 0;
                                }

                                // Soldier strikes back
                                if (soldierhp[n] > 0)
                                {
                                    // Is the human carrying a shield?

                                    // At this point armor is persistent
                                    if (shield == 0)
                                    {
                                        damage = random.Next(1, soldierDamage);
                                        humanhp -= damage;
                                    }
                                    else
                                    {
                                        damage = random.Next(1, soldierDamage);
                                        humanhp -= damage / 2;
                                    }

                                    if (humanhp < 0)
                                    {
                                        humanhp = 0;
                                        endOfGame = true;
                                        break;
                                    }
                            }

                            }
                        }
                        humanx = humanx1;
                        humany = humany1;
                    }
                
                    // Otherwise:
                    // Make previous position empty space
                    // Make current position player
                    flatLand[humanx1, humany1] = emptyspace;
                    flatLand[humanx, humany] = human;

                return;
            }

            void CitizensMove()
            {
                /*
                 * Citizens are docile, mostly wandering at random & doing their best to avoid bumping
                 * into all other characters, including each other.
                 * 
                 * They will retreat from the human if the human gets 'too close' AND is armed.
                 * 'too close': ABS(humanx - citizenx[n]) < 2 AND ABS(humany - citizeny[n]) < 2
                 * 
                 * They will attack if injured by a human.
                 * 
                 * 
                 */

                // Move each citizen in turn.
                for (n = 0; n < numCitizens; n++)
                {
                    // Is the citizen dead? (0 HP)
                    if (citizenhp[n] == 0) { continue; } // Move on to the next citizen
                    
                    // Store the citizen's current position as previous position
                    citizenx1[n] = citizenx[n];
                    citizeny1[n] = citizeny[n];

                    //  *** Look for danger first

                    // Is the human 'too close' AND isArmed?
                    // 'too close': ABS(humanx - citizenx[n]) < 2 AND ABS(humany - citizeny[n]) < 2

                    if ((Math.Abs(humanx - citizenx[n]) < 2 && Math.Abs(humany - citizeny[n]) < 2)
                        && isArmed)
                    {
                        // Human is 'too close' AND isArmed.
                        // Make an effort to move away from the human

                        do
                        {
                            outOfBoundsArmed:
                            citizenx[n] = citizenx1[n];
                            citizeny[n] = citizeny1[n];

                            RandomWalk();

                            // This is the bias away from the human.
                            deltax += Math.Sign(citizenx[n] - humanx);
                            deltay += Math.Sign(citizeny[n] - humany);

                            citizenx[n] += deltax;
                            citizeny[n] += deltay;

                            if (citizenx[n] < 1 || citizenx[n] > maxX - 1 ||
                               citizeny[n] < 1 || citizeny[n] > maxY - 1)
                            { goto outOfBoundsArmed; }
                        }
                        while (flatLand[citizenx[n], citizeny[n]] != emptyspace &&
                               flatLand[citizenx[n], citizeny[n]] != teleport);

                        // or...


                    }
                    else
                    {
                        do
                        {
                            outOfBoundsUnarmed:
                            // Citizen can safely wander.
                            citizenx[n] = citizenx1[n];
                            citizeny[n] = citizeny1[n];

                            RandomWalk();

                            citizenx[n] += deltax;
                            citizeny[n] += deltay;

                            if (citizenx[n] < 1 || citizenx[n] > maxX - 1 ||
                               citizeny[n] < 1 || citizeny[n] > maxY - 1)
                            { goto outOfBoundsUnarmed; }
                        }
                        while (flatLand[citizenx[n], citizeny[n]] != emptyspace &&
                               flatLand[citizenx[n], citizeny[n]] != teleport);
                    }

                    // Is the citizen's requested position occupied by a teleport?
                    if (flatLand[citizenx[n], citizeny[n]] == teleport)
                    {
                        // <!WHOOSH!> Where will they end up?!
                        TeleportCharacter();
                        citizenx[n] = x;
                        citizeny[n] = y;
                    }

                    // Make previous position empty space
                    // Make current position citizen
                    flatLand[citizenx1[n], citizeny1[n]] = emptyspace;
                    flatLand[citizenx[n], citizeny[n]] = citizen;                    
                }
                return;
            }

            void SoldiersMove()
            {
                /*
                 * Soldiers remain stationary until an armed human passes within five cells of their
                 * position. At that point, a soldier will aggressively seek out the human and attack.
                 * 
                */

                // Move each soldier in turn.
                for (n = 0; n < numSoldiers; n++)
                {
                    // Is the soldier dead? (0 HP)
                    if (soldierhp[n] == 0) { continue; } // Move on to the next soldier

                    // Store the soldier's current position as previous position
                    soldierx1[n] = soldierx[n];
                    soldiery1[n] = soldiery[n];

                    // Is the human 'too close' AND isArmed?
                    // 'too close': ABS(humanx - soldierx[n]) < 5 AND ABS(humany - soldiery[n]) < 5

                    if ((Math.Abs(humanx - soldierx[n]) < 5 && Math.Abs(humany - soldiery[n]) < 5)
                        && isArmed)
                    {
                        // Human is 'too close' AND isArmed.
                        // Move toward the human

                        do
                        {
                        outOfBoundsSoldier:
                            soldierx[n] = soldierx1[n];
                            soldiery[n] = soldiery1[n];

                            RandomWalk();

                            // This is the bias toward from the human.
                            deltax += Math.Sign(humanx - soldierx[n]);
                            deltay += Math.Sign(humany - soldiery[n]);

                            soldierx[n] += deltax;
                            soldiery[n] += deltay;

                            if (soldierx[n] < 1 || soldierx[n] > maxX - 1 ||
                               soldiery[n] < 1 || soldiery[n] > maxY - 1)
                            { goto outOfBoundsSoldier; }
                        }
                        while (flatLand[soldierx[n], soldiery[n]] != emptyspace &&
                               flatLand[soldierx[n], soldiery[n]] != teleport);

                        // Is the soldier's requested position occupied by a teleport?
                        if (flatLand[soldierx[n], soldiery[n]] == teleport)
                        {
                            // <!WHOOSH!> Where will they end up?!
                            TeleportCharacter();
                            soldierx[n] = x;
                            soldiery[n] = y;
                        }

                        // Is the soldier's requested position occupied by the human?
                        // Attack the human
                        if (flatLand[soldierx[n], soldiery[n]] == human)
                        {
                            // Is the human carrying a shield?

                            // At this point armor is persistent
                            if (shield == 0)
                            {
                                damage = random.Next(1, soldierDamage);
                                humanhp -= damage;
                            }
                            else
                            {
                                damage = random.Next(1, soldierDamage);
                                humanhp -= damage / 2;
                            }

                            if (humanhp < 0)
                            {
                                humanhp = 0;
                                endOfGame = true;
                                break;
                            }
                        }

                        // Make previous position empty space
                        // Make current position citizen
                        flatLand[soldierx1[n], soldiery1[n]] = emptyspace;
                        flatLand[soldierx[n], soldiery[n]] = soldier;


                    }

                }
                return;
            }

            void RandomWalk()
            {
                    // pick a random direction
                    deltax = random.Next(0, 3) - 1; // (-1 to +1)
                    deltay = random.Next(0, 3) - 1; // (-1 to +1)

                return;
            }
        }
    }
}
