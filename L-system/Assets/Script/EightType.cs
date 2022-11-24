
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EightType : MonoBehaviour
{
    public GameObject TreeParent;
    public GameObject Branch;
    public GameObject Leaf;
    private GameObject Tree;
    private GameObject TreeSegment;
    private GameObject Leaves;

    
    private string CurrentString;
    private string InitialString;

    private float width;
    private float length;
    private float leafwidth;
    private float leaflength;
    private float angle;
    private float solidangle;
    private int iteration;
    private Color StartColor = new Color(169.0f/255.0f, 118.0f/255.0f, 32.0f/255.0f, 100.0f/255.0f);
    private Color EndColor = Color.green;
    
    List<string> RuleList = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8" ,"9"};
    List<GameObject> TreeSegments = new List<GameObject>();
    List<GameObject> LeavesList = new List<GameObject>();

    private Dictionary<char, string> rules;
    private Stack<TransformInfo> transformStack;

    [SerializeField] private InputField IWidth;
    [SerializeField] private InputField ILength;
    [SerializeField] private InputField ILeafWidth;
    [SerializeField] private InputField ILeafLength;
    [SerializeField] private InputField IAngle;
    [SerializeField] private InputField ISolidAngle;
    [SerializeField] private InputField IIteration;
  

    public Button GenerateTree;
    public Button ResetTree;
    public Button StepBack;
    public Button StepForward;
    public Button Come2DIYTree;
    public Slider FOVS;
    public Dropdown RulesSelect;

    void Start()
    {
        Camera.main.fieldOfView = 60.0f;
        transformStack = new Stack<TransformInfo>();
        RulesSelect.ClearOptions();
        RulesSelect.AddOptions(RuleList);
        RulesSelect.onValueChanged.AddListener(SelectRulesF);

        InitialString = "F";
        rules = new Dictionary<char, string>
        {
            { 'F', "F[+F]F[-F]F" }
        };

    }

    void Update()
    {
        OnGUI();  
    }

    

    public void OnGUI()
    {
        FOVS.minValue = 20.0f;
        FOVS.maxValue = 179.0f;
        Camera.main.fieldOfView = FOVS.value;
    }

    private bool isLeaf(string rule, int index)
    {
        if (rule[index] != 'F') return false;
        for (int i = index + 1; i < rule.Length; i++)
        {
            if (rule[i] == ']') return true;
            else if (rule[i] == 'F') return false;
        }
        return false;
    }

    public void InputShape()
    {
        width = float.Parse(IWidth.text);
        length = float.Parse(ILength.text);
        leafwidth = float.Parse(ILeafWidth.text);
        leaflength = float.Parse(ILeafLength.text);
        angle = float.Parse(IAngle.text);
        solidangle = float.Parse(ISolidAngle.text);
        iteration = int.Parse(IIteration.text);

    }
    public void Generate()
    {
        InputShape();
        GenerateByParam(width, length,leafwidth ,leaflength , angle, solidangle, iteration);
    }
    public void GenerateByParam(float width, float length,float leafwidth ,float leaflength , float angle,float solidangle, int iteration)
    {
        Destroy(Tree);
        foreach (GameObject TreeSegment in TreeSegments)
        {
            Destroy(TreeSegment);
        }
        foreach (GameObject Leaves in LeavesList)
        {
            Destroy(Leaves);
        }
        TreeSegments.Clear();
        LeavesList.Clear();
        Tree = Instantiate(TreeParent);
        CurrentString = InitialString;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < iteration; i++)
        {
            foreach (char c in CurrentString)
            {
                sb.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());
                Debug.Log(c);
                Debug.Log(sb);
            }
            CurrentString = sb.ToString();
            Debug.Log(CurrentString);
            Debug.Log(i);
            sb = new StringBuilder ();
        }

        for (int i = 0; i < CurrentString.Length; i++)
        {
            switch (CurrentString[i])
            {
                case 'F':
                    Vector3 initialPosition = transform.position;
                    transform.Translate(Vector3.up * length);
                    if (isLeaf (CurrentString,i))
                    {
                        Leaves = Instantiate(Leaf);
                        LeavesList.Add(Leaves);
                        Leaves.GetComponent<LineRenderer>().SetPosition(0, initialPosition);
                        Leaves.GetComponent<LineRenderer>().SetPosition(1, transform.position);
                        Leaves.GetComponent<LineRenderer>().startWidth = leafwidth;
                        Leaves.GetComponent<LineRenderer>().endWidth = 0.0f;
                        Leaves.GetComponent<LineRenderer>().startColor = EndColor;
                        Leaves.GetComponent<LineRenderer>().endColor = EndColor;
                        Leaves.GetComponent<LineRenderer>().material = new Material(Shader.Find("Sprites/Default"));
                        Leaves.transform.SetParent(Tree.transform);
                        break;
                    }
                    else
                    {
                    TreeSegment = Instantiate(Branch);
                    TreeSegments.Add(TreeSegment);
                    TreeSegment.GetComponent<LineRenderer>().SetPosition(0, initialPosition);
                    TreeSegment.GetComponent<LineRenderer>().SetPosition(1, transform.position);
                    TreeSegment.GetComponent<LineRenderer>().startWidth = width;
                    TreeSegment.GetComponent<LineRenderer>().endWidth = width;
                    TreeSegment.GetComponent<LineRenderer>().startColor = StartColor;
                    TreeSegment.GetComponent<LineRenderer>().endColor = StartColor;
                    TreeSegment.GetComponent<LineRenderer>().material = new Material(Shader.Find("Sprites/Default"));
                    TreeSegment.transform.SetParent(Tree.transform);
                    break;
                    }
                case 'X':
                    break;
                case '+':
                    transform.Rotate(Vector3.forward * angle );
                    break;
                case '-':
                    transform.Rotate(Vector3.back * angle );
                    break;
                case '/':
                    transform.Rotate(Vector3.up * solidangle );
                    break;
                case '*':
                    transform.Rotate(Vector3.down * solidangle );
                    break;
                case '[':
                    transformStack.Push(new TransformInfo()
                    {
                        position = transform.position,
                        rotation = transform.rotation
                    });
                    break;
                case ']':
                    {
                        TransformInfo ti = transformStack.Pop();
                        transform.position = ti.position;
                        transform.rotation = ti.rotation;
                    }
                    break;
                default:
                    throw new InvalidOperationException("Invalid L-tree operation");
            }
        }
        
    }

    private void SelectTreeOne()
    {
        //‘⁄UI÷–œ‘ æ2∫≈
        rules.Clear();
        InitialString = "F";
        rules = new Dictionary<char, string>
        {
            { 'F', "F[+F]F[-F][F]" }
        };
    }

    private void SelectTreeTwo()
    {
        rules.Clear();
        InitialString = "F";
        rules = new Dictionary<char, string>
        {
            { 'F', "FF-[-F+F+F]+[+F-F-F]" }
        };
    }

    private void SelectTreeThree()
    {
        rules.Clear();
        InitialString = "X";
        rules = new Dictionary<char, string>
        {
            { 'X', "F[+X]F[-X]+X" },
            { 'F', "FF" }
        };
    }

    private void SelectTreeFour()
    {
        rules.Clear();
        InitialString = "X";
        rules = new Dictionary<char, string>
        {
            { 'X', "F[+X][-X]FX" },
            { 'F', "FF" }
        };
    }

    private void SelectTreeFive()
    {
        rules.Clear();
        InitialString = "X";
        rules = new Dictionary<char, string>
        {
            { 'X', "F-[[X]+X]+F[+FX]-X" },
            { 'F', "FF" }
        };
    }

    private void SelectTreeSix()
    {
        rules.Clear();
        InitialString = "X";
        rules = new Dictionary<char, string>
        {
            { 'X', "[F/[+FX][*+FX][/+FX]*F]" },
            { 'F', "FF" }
        };
    }

    private void SelectTreeSeven()
    {
        rules.Clear();
        InitialString = "X";
        rules = new Dictionary<char, string>
        {
            { 'X', "[*+FX]X*[+FX][/+*F-FX//F]" },
            { 'F', "FF" }
        };
    }

    private void SelectTreeEight()
    {
        rules.Clear();
        InitialString = "X";
        rules = new Dictionary<char, string>
        {
            { 'X', "[F[-X+F[+FX]][*-X+F[+FX]][/-X+F[+FX]-X]]" },
            { 'F', "FF" }
        };
    }
    private void SelectTreeNine()
    {
        rules.Clear();
        InitialString = "X";
        rules = new Dictionary<char, string>
        {
            { 'X', "[F-[[X]+X]+F[+FX]-X]" },
            { 'F', "FF" }
        };
    }

    public void SelectRulesF(int i)
    {
        switch (i)
        {
            case 1:
                SelectTreeOne();
                break;
            case 2:
                SelectTreeTwo();
                break;
            case 3:
                SelectTreeThree();
                break;
            case 4:
                SelectTreeFour();
                break;
            case 5:
                SelectTreeFive();
                break;
            case 6:
                SelectTreeSix();
                break;
            case 7:
                SelectTreeSeven();
                break;
            case 8:
                SelectTreeEight();
                break;
            case 9:
                SelectTreeNine();
                break;
        }

        foreach (var rule in rules.Values)
        {
            Debug.Log(string.Format("in rule :\"{0}\"", rule));
            for (int j = 0; j < rule.Length; j++)
            {
                if (isLeaf(rule, j))
                {
                    Debug.Log(string.Format("Find leaf in #{0}", j));
                }
            }
        }
    }

    public void StepBackF()
    {
        //Destroy(Tree);
        //foreach (GameObject TreeSegment in TreeSegments)
        //{
        //    Destroy(TreeSegment);
        //}
        //foreach (GameObject Leaves in LeavesList)
        //{
        //    Destroy(Leaves);
        //}
        //TreeSegments.Clear();
        //LeavesList.Clear();
        if (iteration <= 0)
        {
            return;
        }
        iteration--;
        IIteration.text = iteration.ToString();
        GenerateByParam(width, length, leafwidth, leaflength, angle, solidangle, iteration);
        Debug.Log("done!");
    }

    public void StepForwardF()
    {
        //Destroy(Tree);
        //foreach (GameObject TreeSegment in TreeSegments)
        //{
        //    Destroy(TreeSegment);
        //}
        //foreach (GameObject Leaves in LeavesList)
        //{
        //    Destroy(Leaves);
        //}
        //TreeSegments.Clear();
        //LeavesList.Clear();
        Debug.Log("done!");
        iteration++;
        IIteration.text = iteration.ToString();
        GenerateByParam(width, length, leafwidth, leaflength, angle, solidangle, iteration);
    }

    public void Come2DIYTreeF()
    {
        rules.Clear();
        Destroy(Tree);
        foreach (GameObject TreeSegment in TreeSegments)
        {
            Destroy(TreeSegment);
        }
        foreach (GameObject Leaves in LeavesList)
        {
            Destroy(Leaves);
        }
        TreeSegments.Clear();
        LeavesList.Clear();
        SceneManager.LoadScene(0);
    }

    public void OnEnable()
    {
        GenerateTree.onClick.AddListener(Generate);
        StepBack.onClick.AddListener(StepBackF);
        StepForward.onClick.AddListener(StepForwardF);
        Come2DIYTree.onClick.AddListener(Come2DIYTreeF);
    }

}
