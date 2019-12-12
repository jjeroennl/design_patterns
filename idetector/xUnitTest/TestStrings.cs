using System;
using System.Collections.Generic;
using System.Text;

namespace xUnitTest
{
    static class TestStrings
    {
        public static string RandomClasses()
        {
            return @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using PianoTrainer.Model;

namespace PianoTrainer.Controller
{
    class KeyColourController
    {

        //controller recolours the keys draw by the DrawKey(); function every tick
        //Uses the LightKeyUp(); function's list to determine what keys are active
        //this function does not draw keys, only changes colours for existing keys.
        public void ColourKeys(List<KeyboardNoteModel> keyboardKeys)
        {
            foreach (var key in keyboardKeys.OfType<KeyboardNoteModel>())
            {
                if (key.Colour.Equals(""white""))
                {
                    if (key.Active)
                    {
                        key.Rectangle.Fill = Brushes.Green;
                    }
                    else if (!key.Active)
                    {
                        key.Rectangle.Fill = Brushes.White;
                    }
                }

                if (key.Colour.Equals(""black""))
                {
                    if (key.Active)
                    {
                        key.Rectangle.Fill = Brushes.Green;
                    }
                    else if (!key.Active)
                    {
                        key.Rectangle.Fill = Brushes.Black;
                    }
                }

            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PianoTrainer.Model;

namespace PianoTrainer.Controller
{
   public class LightKeyboardKeyUpController
    {
        public List<KeyboardNoteModel> KeyboardKeys;
        
        public void LightKeyUp(List<MusicNoteModel> _activeNoteList)
        {
            //search for notes
            foreach (var item in _activeNoteList)
            {
                //search for keys
                foreach (var keyboardKey in KeyboardKeys)
                {
                    //if the pitch from the note and the pitch from the key are the same && 
                    //if the chromatic from the note ande the chromatic from the key are the same && 
                    //if the octave from the note and the octave from the key are the same keyboardKey.Active = true
                    if (item.PitchString.Equals(keyboardKey.Pitch.ToString()) &&
                        item.ChromaticString.ToLower().Equals(keyboardKey.ChromaticString.ToLower()) &&
                        item.Octave == keyboardKey.Octave)
                    {

                        keyboardKey.Active = true;
                    }
                    else
                    {
                        keyboardKey.Active = false;

                    }
                }
            }
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using PianoTrainer.Model;
using PianoTrainer.View;

namespace PianoTrainer.Controller
{
	 public class MetronomeController
	{
		public MetronomeModel MetronomeModel;
        public LightKeyboardKeyUpController LightUp;
        public List<MusicNoteModel> MusicNotes;
        public List<MusicNoteModel> ActiveNoteList = new List<MusicNoteModel>();
        public MusicNoteController NoteController { get; set; }
		private List<Line> _staff = new List<Line>();

		private int _staffHght = 10;
		private int _metronomelength = 14 * 11;

        public MetronomeController(MetronomeModel model)
		{
			MetronomeModel = model;
		}

        public void MoveMetronome(double width)
        {

           MetronomeModel.X1 += 2;
            MetronomeModel.X2 += 2;
            // if the line reaches the end of the canvas it'll drop down to the next staffs
            if (MetronomeModel.Metronome.X1 > width)
            {
                MetronomeModel.StartScroll = true;
                MetronomeModel.Y1 = MetronomeModel.Y2;
                MetronomeModel.Y2 = MetronomeModel.Y2 + _metronomelength;

                MetronomeModel.X1 = 0;
                MetronomeModel.X2 = 0;
            }
        }

		// draws the static components for the first time
		public List<Line> DrawStave(double width, KeyboardSheet keyboard)
		{
			//sets the spaces for the music staffs
			//----------------------------
			for (int i = 0; i < keyboard.Lines + 1; i++)
			{
				int SndStaff = 0;

				SndStaff += i * MetronomeModel.Stave;
				for (int j = 1; j < 11; j++)
				{
					if (j == 6)
					{
						SndStaff += 20;
					}
					else if (j == 11)
					{
						SndStaff += 30;
					}
					//-----------------------------------
					Line line = new Line();
					line.Stroke = System.Windows.Media.Brushes.Black;
					line.X1 = 0;
					line.X2 = width;
					line.Y1 = j * _staffHght + SndStaff + 8;
					line.Y2 = j * _staffHght + SndStaff + 8;

					line.StrokeThickness = 2;

					_staff.Add(line);
				}
			}
			foreach (Line item in _staff)
			{
				Console.WriteLine(item.Y1);
			}
			return _staff;
		}

		//updates the the current position of the _metronome
		public void Repaint()
		{
			MetronomeModel.Metronome.X1 = MetronomeModel.X1;
			MetronomeModel.Metronome.X2 = MetronomeModel.X2;
			MetronomeModel.Metronome.Y1 = MetronomeModel.Y1;
			MetronomeModel.Metronome.Y2 = MetronomeModel.Y2;
		}

		public void CheckMetronomePosition()
        {
            for (int i = 0; i < MusicNotes.Count; i++)
            {
                //If the x pos of the _metronome is equal of higher than the x pos of i && 
                //if the x pos of the _metronome is smaller than x pos of i +10
                //check if note pos is inside metronomes pos
                if (MetronomeModel.Metronome.X1 >= MusicNotes[i].PositionX &&
                    MetronomeModel.Metronome.X1 < MusicNotes[i].PositionX + 10 &&
                    !ActiveNoteList.Contains(MusicNotes[i]) &&
                    NoteController.SetPitch(MusicNotes[i].PitchString) >= MetronomeModel.Metronome.Y1 &&
                    NoteController.SetPitch(MusicNotes[i].PitchString) <= MetronomeModel.Metronome.Y2)
                {
                    //Add tempList to _activeNoteList
                    ActiveNoteList.Add(MusicNotes[i]);
                }
                else
                {
                    //Remove tempList from _activeNoteList
                    ActiveNoteList.Remove(MusicNotes[i]);
                }
            }
            LightUp.LightKeyUp(ActiveNoteList);
        }

        // draws the static components for the first time
        public Line DrawMetronome()
		{
			MetronomeModel.Y1 = 0;
			MetronomeModel.Y2 = 14.5 * _staffHght;

			MetronomeModel.Metronome.Stroke = System.Windows.Media.Brushes.Red;
			MetronomeModel.Metronome.X1 = MetronomeModel.X1;
			MetronomeModel.Metronome.X2 = MetronomeModel.X2;
			MetronomeModel.Metronome.Y1 = MetronomeModel.Y1;
			MetronomeModel.Metronome.Y2 = MetronomeModel.Y2;

			MetronomeModel.Metronome.StrokeThickness = 4;

			return MetronomeModel.Metronome;
		}

        //Function to reset te metronome so it starts again.
        public void RestartMetronome()
        {
            MetronomeModel.X1 = 0;
            MetronomeModel.X2 = 0;
            MetronomeModel.Y1 = 0;
            MetronomeModel.Y2 = 14.5 * 10;
        }
	}
}

using PianoTrainer.Database;
using PianoTrainer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PianoTrainer.Controller
{
    public class MusicNoteController
    {
        public List<MusicNoteModel> Notes = new List<MusicNoteModel>();
        private Pitch enumPitch;
        private KeyboardSheet _sheet;

        public List<MusicNoteModel> SetAllNotes(Model.KeyboardSheet sheet)
        {
            _sheet = sheet;
            //getting info out of database
            using (var db = new MyContext())
            {
                //gets all items out of stavesRight 
                var query = db.StavesRight
                    // where keyboardSheetId is equel to given keyboardsheet
                    .Where(s => s.KeyboardSheetId == sheet.KeyboardSheetId)
                    //orderby positionX
                    .OrderBy(s => s.PositionX)
                    .AsEnumerable()
                    //adds database table Notes based on keyboardNoteId so we have all info of the notes
                    .Join(db.Notes, p => new { p1 = p.KeyboardNoteId }, e => new { p1 = e.KeyboardNoteId },
                    //the items it pulls out of the item
                    (p, e) => new MusicNoteModel
                    {
                        Id = p.KeyboardNoteId,
                        PositionX = p.PositionX,
                        Octave = e.Octave,
                        PitchString = e.PitchString,
                        ChromaticString = e.ChromaticString,
                        HasLine = true
                    })
                    //put it in a list
                    .ToList();

                foreach (MusicNoteModel note in query)
                {
                    Enum.TryParse(note.PitchString, out enumPitch);
                    note.PositionX = SetNoteX(note);
                    note.PositionY = SetNoteY(note);
                    note.HasLine = HasLine(note);
                    Notes.Add(note);
                }

                return Notes;
            }
        }

        public double SetNoteY(Model.MusicNoteModel note)
        {
            double y = 143.7;

            //y -= (note.Octave - _startOctave) * 35;
            y -= (note.Octave - 2) * 35;
            y -= SetPitch(note.PitchString);
            //y += _sheet.Lines * _stave;
            y += _sheet.Lines * 150;

            return y;
        }

        public double SetNoteX(Model.MusicNoteModel note)
        {
            double x = note.PositionX;

            if (x > _sheet.MaxWidth)
            {
                if (x > _sheet.MaxWidth)
                {
                    _sheet.Lines = (int)Math.Floor(x / _sheet.MaxWidth);
                }

                x -= (int)((_sheet.MaxWidth) * _sheet.Lines);
            }

            return x;
        }

        public bool HasLine(Model.MusicNoteModel note)
        {
            //if (_musicNote.Octave == _startOctave && (int) enumPitch < 1 || _musicNote.Octave > _startOctave + 1 && (int) enumPitch < 1)
            if (note.Octave == 2 && (int)enumPitch < 1 || note.Octave > 2 + 1 && (int)enumPitch < 1)
            {
                return false;
            }

            return true;
        }


        public int SetPitch(string pitch)
        {
            int y = 0;

            switch (enumPitch)
            {
                case Pitch.B:
                    y = 5;
                    break;
                case Pitch.C:
                    y = 10;
                    break;
                case Pitch.D:
                    y = 15;
                    break;
                case Pitch.E:
                    y = 20;
                    break;
                case Pitch.F:
                    y = 25;
                    break;
                case Pitch.G:
                    y = 30;
                    break;
            }

            return y;
        }
    }
}
namespace PianoTrainer.Controller{
public interface INote {
   INote getNote();
    
   void setNote(INote note):
    }
}

namespace PianoTrainer.Controller {
public class NoteA : INote{
        INote _note;

        public INote getNote(){
            return note;
        }
        public void SetNote(INote note){
            _note = note;
        }
    }
}
namespace PianoTrainer.Controller {
public class NoteB : INote{
        INote _note;

        public INote getNote(){
            return note;
        }
        public void SetNote(INote note){
            _note = note;
        }
    }
}
public class NoteBs : NoteB{
        INote _note;

        public INote getNote(){
            return note;
        }
        public void SetNote(INote note){
            _note = note;
        }
    }
}";
        }

        public static string CorrectDecoratorString()
        {
            return
                "public abstract class ComponentBase\r\n{\r\n    public abstract void Operation();\r\n}\r\n \r\n \r\npublic class ConcreteComponent : ComponentBase\r\n{\r\n    public override void Operation()\r\n    {\r\n        Console.WriteLine(\"Component Operation\");\r\n    }\r\n}\r\n \r\n \r\npublic abstract class DecoratorBase : ComponentBase\r\n{\r\n    private ComponentBase _component;\r\n \r\n    public DecoratorBase(ComponentBase component)\r\n    {\r\n        _component = component;\r\n    }\r\n \r\n    public override void Operation()\r\n    {\r\n        _component.Operation();\r\n    }\r\n}\r\n \r\n \r\npublic class ConcreteDecorator : DecoratorBase\r\n{\r\n    public ConcreteDecorator(ComponentBase component) : base(component) { }\r\n \r\n    public override void Operation()\r\n    {\r\n        base.Operation();\r\n        Console.WriteLine(\"(modified)\");\r\n    }\r\n}";
        }
    }
}
