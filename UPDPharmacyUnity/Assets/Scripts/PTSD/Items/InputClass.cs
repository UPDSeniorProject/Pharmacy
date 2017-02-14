using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Text;

[XmlRoot("InputXML")]
public class InputClass {
    [XmlAttribute("IntroText")]
    public string IntroText;

    [XmlAttribute("TasksDescription")]
    public string Scenario;

    [XmlElement("BumpEncounter")]
    public BumpEncounter bumpEncounter = new BumpEncounter();

    [XmlElement("NPCOption")]
    public NPCOption npcOption = new NPCOption();

   // [XmlElement("SpecialNPCList")]
    [XmlArray("SpecialNPCList")]
    [XmlArrayItem("NPCName")]
    public List<string> specialNPCList = new List<string>();

    //[XmlElement("ShopperConversations")]
    [XmlArray("ShopperConversations")]
    [XmlArrayItem("IncludedConversation")]
    public List<ShopperConversationsTypes> shopperConversations = new List<ShopperConversationsTypes>();

    [XmlArray("Tasks")]
    [XmlArrayItem("Task")]
    public List<Task> tasks = new List<Task>();

    [XmlArray("PickableItems")]
    [XmlArrayItem("PickableItem")]
    public List<Item> pickableItems = new List<Item>();

    [XmlArray("ShoppingListItems")]
    [XmlArrayItem("ShoppingListItem")]
    public List<Item> shoppingListItems = new List<Item>();

	[XmlArray("TutorialSteps")]
	[XmlArrayItem("TutorialStep")]
	public List<TutorialStep> tutorialSteps = new List<TutorialStep> ();

	[XmlElement("VirtualCheckoutCashier")]
	public string VirtualCheckoutCashier;

	[XmlArray("Distractions")]
	[XmlArrayItem("IncludedDistraction")]
	public List<string> distractions = new List<string>();

    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(InputClass));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            var xmlWriter = new XmlTextWriter(stream, Encoding.UTF8);
            serializer.Serialize(xmlWriter, this);
            //serializer.Serialize(stream, this);
        }
    }

    public static InputClass Load(string path)
    {
        var serializer = new XmlSerializer(typeof(InputClass));
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return serializer.Deserialize(stream) as InputClass;
        }
    }

    //Loads the xml directly from the given string. Useful in combination with www.text.
    public static InputClass LoadFromText(string text)
    {
        var serializer = new XmlSerializer(typeof(InputClass));
        return serializer.Deserialize(new StringReader(text)) as InputClass;
    }
}
