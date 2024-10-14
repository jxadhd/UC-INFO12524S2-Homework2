/*

 __     __     ______     __         ______     ______     __    __     ______    
/\ \  _ \ \   /\  ___\   /\ \       /\  ___\   /\  __ \   /\ "-./  \   /\  ___\  
\ \ \/ ".\ \  \ \  __\   \ \ \____  \ \ \____  \ \ \/\ \  \ \ \-./\ \  \ \  __\  
 \ \__/".~\_\  \ \_____\  \ \_____\  \ \_____\  \ \_____\  \ \_\ \ \_\  \ \_____\ 
  \/_/   \/_/   \/_____/   \/_____/   \/_____/   \/_____/   \/_/  \/_/   \/_____/                                                                                                                                                            

     
Kia ora! Haere mai, tauti mai.
Welcome to my submission for Assignment 2. I enjoyed this assignment a lot - thank you Tanja, Gary & Gary :)
Bring on the exam - I'm hoping after today you will agree I am ready to carve up anything you guys can throw at me >:)
Quite a shame really, not much left of the Semester now. I have really gained a lot from INFO125.
Thank you very much for all the knowledge you have imparted. 

                                                                        - Josh
*/

namespace jcr168_Homework2
{
    public partial class Main : Form
    {
        // Create a global list variable of object type Cruise to store cruise details
        private readonly List<Cruise> cruises = new();

        // Initialise a couple strings as global variables
        private string? SelectedShip;
        string? Ship;

        public Main()
        {
            // Initialize the program and call the file-loading method to kick things off immediately
            InitializeComponent();
            CruiseLoader();
        }

        static List<Cruise> ShipCruises = new();
        static List<GroupedCruise> GroupedCruises = new();

        // Select the .txt cruise data file
        private string FileSelect()
        {
            // Force the user to select the structured .txt file the program will reference, but not before we
            // welcome them to our software and let them gently into dlgOpenFile
            MessageBox.Show("Please select a cruise data file.", "Welcome!");

            // If the user fails to select a file, they are not considered worthy
            if (dlgOpenFile.ShowDialog() == DialogResult.Cancel)
            {
                Environment.Exit(0);
            }
            // If the user behaves and selects a file, assign their chosen file path to FilePath and continue through CruiseLoader()
            
            string FilePath = dlgOpenFile.FileName;
            return FilePath;
        }

        // Load the data file into memory, so it can be presented to the user later
        private void CruiseLoader()
        {
            // Parses and stores data from the appropriate file.
            try
            {
                // Prompt the user to select a file, then:
                // Open the file stream for reading, putting "using" on the front conveniently makes it close
                // automatically
                using StreamReader file = new(FileSelect());


                // Bring our form to the front, because for some reason it goes to the back after FileSelect
                // With either of these two lines commented notice it goes straight to the back of the window stack
                // Oh well, this fixes it regardless
                // In development, it still sometimes went to back but seems fine now
                this.Show();
                this.Activate();


                // Declare nullable string to store each line - in this case, 1 line = 1 data entry
                // Doing this enables us to give it a little class later on
                string? line;
                while ((line = file.ReadLine()) != null)
                {
                    string[]? data = line.Split(',');
                    if (DateTime.TryParseExact(data[4], "dd-MMM-yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime startDate))
                    {
                        string getName = "";
                        string shipnum = data[1];
                        if (shipnum == "13")
                        {
                            getName = "Aurora";
                        }
                        else if (shipnum == "15")
                        {
                            getName = "Arcadia";
                        }
                        else if (shipnum == "16")
                        {
                            getName = "Adonia";
                        }
                        else if (shipnum == "17")
                        {
                            getName = "Oceana";
                        }
                        else if (shipnum == "18")
                        {
                            getName = "Grand Princess";
                        }
                        else if (shipnum == "20")
                        {
                            getName = "Oriana";
                        }
                        else if (shipnum == "23")
                        {
                            getName = "Dawn Princess";
                        }

                        Cruise cruise = new()
                        {
                            Number = data[0],
                            ShipNumber = data[1],
                            Duration = data[2],
                            Cost = int.Parse(data[3]).ToString("C"),
                            StartDate = startDate.ToString("dd-MMM-yyyy"),
                            Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(data[5]),
                            ShipName = getName,
                        };
                        cruises.Add(cruise);
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show($"An unhandled exception occurred:\n{error.Message}", "Error :(");
                Environment.Exit(1);
            }
        }



        // Takes structured data from file previously read into memory and parses it to get it ready for printing!
        private void cruiseFinder(string shipNumber)
        {

            // Where the ship number of our cruise is equal to the one we're looking for,
            // add that scheduled cruise to a list...
            ShipCruises = cruises.Where(c => c.ShipNumber == shipNumber).ToList<Cruise>();

            // And group it by cruise name, before sorting by departure date and sending this to list GroupedCruises
            GroupedCruises = ShipCruises
                .GroupBy(c => c.Name)
                .Select(g => new GroupedCruise
                {
                    CruiseName = g.Key,
                    Sailings = g.OrderBy(c => DateTime.ParseExact(c.StartDate, "dd-MMM-yyyy",
                    CultureInfo.InvariantCulture)).ToList()
                })
                .ToList<GroupedCruise>();

            ShipCruises.Sort();

            // ShipCruises.Sort()
            // that doesn't work
            List<string>? sortedGroupedCruises = GroupedCruises.Select(g => g.CruiseName).ToList();
            sortedGroupedCruises.Sort();

            // Configure combobox, as it will very shortly be usable
            cbxCruiseSelect.Enabled = true;

            // Approaching the end of our cruise finding function, cbxCruiseSelect becomes a dropdown box of each
            // unique cruise sorted alphabetically, for the selected ship only.
            cbxCruiseSelect.DataSource = sortedGroupedCruises;
            cbxCruiseSelect.Text = $"Select a cruise aboard {Ship}";

            DisplayCruises(ShipCruises);
        }

        // Takes the variables we parsed out of our data, and prints them in a nice readable fashion for the user :)
        private void DisplayCruises(List<Cruise> cruisesToDisplay)
        {
            // StringBuilder makes building the string for this nice and simple
            StringBuilder details = new();

            // Present the user with some key data, then leave a line and print the lines of user-selected data
            details.AppendLine($"Displaying informaton for {cruisesToDisplay.Count} of " +
                $"{cruises.Count} total confirmed sailings.");
            details.AppendLine();
            // Exception handling, although this should not be necessary at this point
            if (cruisesToDisplay.Count == 0)
            {
                details.AppendLine("No cruises found for the selected ship/cruise combination." +
                    "Try a different ship!.");
            }
            else
            {
                List<GroupedCruise>? cruiseFiltered = cruisesToDisplay
                    .GroupBy(c => c.Name)
                    .Select(g => new GroupedCruise
                    {
                        CruiseName = g.Key,
                        Sailings = g.OrderBy(c => DateTime.ParseExact(c.StartDate, "dd-MMM-yyyy",
                        CultureInfo.InvariantCulture)).ToList()
                    })
                    .ToList();

                details.AppendLine($"{Ship} has {ShipCruises.Count} planned sailings, on " +
                    $"{GroupedCruises.Count} unique routes! " +
                    $"Check out our awesome cruise options aboard {Ship} below, or use the drop-down above to " +
                    $"choose a specific cruise.");
                details.AppendLine();

                foreach (GroupedCruise? cruises in cruiseFiltered)
                {
                    details.AppendLine($"{cruises.CruiseName} - {cruises.Sailings.Count} sailing(s) aboard {Ship}");
                    // Before adding information for each sailing
                    foreach (Cruise? cruise in cruises.Sailings)
                    {
                        details.AppendLine($"Start Date: {cruise.StartDate}");
                        details.AppendLine($"Duration: {cruise.Duration} days");
                        details.AppendLine($"Cost: {cruise.Cost}");
                        details.AppendLine($"");
                    }
                }
            }
            txtCruiseDetails.Text = details.ToString();
        }

        // cbxCruiseSelect selection change handler
        private void cbxCruiseSelect_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // Fancy Windows click sound from the 1990s
            sndClick();
            string? selectedCruiseName = cbxCruiseSelect.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedCruiseName))
            {
                FilterCruises(selectedCruiseName);
            }
        }

        // Method to filter cruises based on the selected cruise name and ship name
        private void FilterCruises(string cruiseName)
        {
            List<Cruise>? filteredCruises = cruises.Where(c => c.Name.Equals(cruiseName) &&
            c.ShipNumber.Equals(SelectedShip)).ToList();

            DisplayCruises(filteredCruises);
        }

        // Initialises a simple "click" sound reminiscent of early Win :)
        private static void sndClick()
        {
            SoundPlayer click = new("click.wav");
            click.Play();
        }

        // Exit button click event
        private void btnExit_Click(object sender, EventArgs e)
        {
            // Play some fancy Windows sounds
            sndClick();
            SystemSounds.Exclamation.Play();

            // Make sure the user really wants to Exit
            DialogResult exitConfirm = MessageBox.Show("Are you sure you want to exit?", "Confirmation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (exitConfirm == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
        }

        // About button click event
        private void btnAbout_Click(object sender, EventArgs e)
        {
            // Fancy Windows click sound from the 1990s
            sndClick();
            // About/copyright page
            Form about = new About();
            about.Show();
        }

        // Ship button click events
        private void btnAurora_Click(object sender, EventArgs e)
        {
            // Fancy Windows click sound from the 1990s
            sndClick();
            // Set SelectedShip to the necessary parameters
            SelectedShip = "13";
            Ship = "Aurora";
            // Find cruises aboard Aurora
            cruiseFinder(SelectedShip);
        }

        private void btnArcadia_Click(object sender, EventArgs e)
        {
            // Fancy Windows click sound from the 1990s
            sndClick();
            // Set SelectedShip to the necessary paremeters
            SelectedShip = "15";
            Ship = "Arcadia";
            // Find cruises aboard Arcadia
            cruiseFinder(SelectedShip);
        }
        private void btnAdonia_Click(object sender, EventArgs e)
        {
            // Fancy Windows click sound from the 1990s
            sndClick();
            // Set SelectedShip to the necessary parameters
            SelectedShip = "16";
            Ship = "Adonia";
            // Find cruises aboard Adonia
            cruiseFinder(SelectedShip);
        }

        private void btnOceana_Click(object sender, EventArgs e)
        {
            // Fancy Windows click sound from the 1990s
            sndClick();
            // Set SelectedShip to the necessary parameters
            SelectedShip = "17";
            Ship = "Oceana";
            // Find cruises aboard Oceana
            cruiseFinder(SelectedShip);
        }

        private void btnGrand_Click(object sender, EventArgs e)
        {
            // Fancy Windows click sound from the 1990s
            sndClick();
            // Set SelectedShip to the necessary parameters
            SelectedShip = "18";
            Ship = "Grand Princess";
            // Find cruises aboard Grand Princess
            cruiseFinder(SelectedShip);
        }

        private void btnOriana_Click(object sender, EventArgs e)
        {
            // Fancy Windows click sound from the 1990s
            sndClick();
            // Set SelectedShip to the necessary parameters
            SelectedShip = "20";
            Ship = "Oriana";
            // Find cruises aboard Oriana
            cruiseFinder(SelectedShip);
        }

        private void btnDawn_Click(object sender, EventArgs e)
        {
            // Fancy Windows click sound from the 1990s
            sndClick();
            // Set SelectedShip to the necessary parameters
            SelectedShip = "23";
            Ship = "Dawn Princess";
            // Find cruises aboard Dawn Princess
            cruiseFinder(SelectedShip);
        }

        // User clicks anywhere on the cbxCruiseSelect control
        private void cbxCruiseSelect_MouseDown(object sender, MouseEventArgs e)
        {
            cbxCruiseSelect.DroppedDown = true;
        }
    }

#pragma warning disable CS8618 // Disable nullability error to keep these classes clean

    // Cruise class to hold our "Cruise" objects
    public class Cruise : IComparable<Cruise>
    {

        public string Number { get; set; }
        public string ShipNumber { get; set; }
        public string Duration { get; set; }
        public string Cost { get; set; }
        public string StartDate { get; set; }
        public string Name { get; set; }
        public string ShipName { get; set; }

        // Implementing CompareTo allows us to use List<Cruise>.Sort(), as we do at Line 173
        public int CompareTo(Cruise? other)
        {
            if (other == null)
            {
                return 1;
            }

            if (this.Name == null)
            {
                return -1;
            }

            if (other.Name == null)
            {
                return 1;
            }

            int result = string.Compare(this.Name, other.Name, StringComparison.Ordinal);
            return result;
        }
    }

    // Originally I used a List<Cruise> to hold this but as the program grew this became necessary
    public class GroupedCruise
    {
        public string CruiseName { get; set; }
        public List<Cruise> Sailings { get; set; }
    }

#pragma warning restore CS8618
}
