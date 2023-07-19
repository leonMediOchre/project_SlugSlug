using Godot;
using System;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;

public partial class DialogueBox : Control {
    private Node _vBox;
    private static Label _nameLabel;
    private static Label _textLabel;
    private static Dictionary<string, Dialogue> _dialogues;
    private static Dialogue _currentDialogue;
    private static int _lineIndex;
    private static List<OptionButton> _optionButtons = new();

    private class Dialogue {
        public DialogueLine[] Lines {get; set;}
        public DialogueOption[] Options {get; set;}  
    }

    private class DialogueLine {
        public string Character {get; set;}
        public string Text {get; set;}
    }

    private class DialogueOption {
        public string ID {get; set;}
        public string Text {get; set;}
    }

    public override void _Ready() {
        _vBox = GetNode("MainPanel/VBox");
        _nameLabel = _vBox.GetNode("NameLabel") as Label;
        _textLabel = _vBox.GetNode("TextPanel/TextLabel") as Label;

        DeserialiseDialogue("Dialogue");
    }

    public override void _UnhandledInput(InputEvent @event) {
        if (@event.IsActionPressed("ui_accept"))
            Next();
    }

    public static void DeserialiseDialogue(string dialogueId) {
        String jsonString = File.ReadAllText($"Assets/Dialogue/{dialogueId}.slug");
        _dialogues = JsonSerializer.Deserialize<Dictionary<string, Dialogue>>(jsonString);

        _currentDialogue = _dialogues["Start"];
        _lineIndex = 0;

        _nameLabel.Text = _currentDialogue.Lines[0].Character;
        _textLabel.Text = _currentDialogue.Lines[0].Text;
    }

    private void Next() {
        if (_currentDialogue.Lines.Length > ++_lineIndex) {
            _nameLabel.Text = _currentDialogue.Lines[_lineIndex].Character;
            _textLabel.Text = _currentDialogue.Lines[_lineIndex].Text;
            GetTree().CallGroup("Characters", "GetCharacterPosition", _nameLabel.Text);
        } else if (_currentDialogue.Options != null)
            foreach (var option in _currentDialogue.Options) {
                OptionButton newButton = new(option.ID);
                newButton.Text = option.Text;
                newButton.Connect("button_down", new Callable(newButton, "OnClick"));
                _optionButtons.Add(newButton);
                _vBox.AddChild(newButton);                           
            }
    }

    private partial class OptionButton : Button {
        private string _optionID;

        public OptionButton(string optionID) : base() {
            _optionID = optionID;
        }

        public void OnClick() {
            _currentDialogue = _dialogues[_optionID];
            _lineIndex = 0;
            _nameLabel.Text = _currentDialogue.Lines[0].Character;
            _textLabel.Text = _currentDialogue.Lines[0].Text;

            _optionButtons.ForEach(o => o.QueueFree());            
            _optionButtons.Clear();
        }
    }
}
