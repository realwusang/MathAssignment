
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using UnityEngine.UI;

using UnityEngine.SceneManagement;


public class GenerationLTree : MonoBehaviour
{
    public GameObject TreeParent;
    public GameObject Branch;
    public GameObject Leaf;
    private GameObject Tree;
    private GameObject TreeSegment;
    private GameObject Leaves;

    private string Axiom;
    private string CurrentString = string.Empty;
    private string Rules_Key;
    private string Rules_Value;

    private float width;
    private float length;
    private float leafwidth;
    private float leaflength;
    private float angle;
    private float solidangle;
    private int iteration;
    private Color StartColor = new Color(169.0f / 255.0f, 118.0f / 255.0f, 32.0f / 255.0f, 100.0f / 255.0f);
    private Color EndColor = Color.green;
 
    List<GameObject> TreeSegments = new List<GameObject>();
    List<GameObject> LeavesList = new List<GameObject>();

    private Dictionary<char, string> rules;
    private Stack<TransformInfo> transformStack;

    [SerializeField] private InputField IAxiom;
    [SerializeField] private InputField IWidth;
    [SerializeField] private InputField ILength;
    [SerializeField] private InputField ILeafWidth;
    [SerializeField] private InputField ILeafLength;
    [SerializeField] private InputField IAngle;
    [SerializeField] private InputField ISolidAngle;
    [SerializeField] private InputField IIteration;
    [SerializeField] private InputField IRules_Key;
    [SerializeField] private InputField IRules_Value;

    public Button GenerateTree;
    public Button ResetTree;
    public Button GenerateAxiom;
    public Button ResetAxiom;
    public Button GenerateRules;
    public Button ResetRules;
    public Button StepBack;
    public Button StepForward;
    public Button Come2EightTree;
    public Slider FOVS;

    void Start()
    {
        Camera.main.fieldOfView = 60.0f;
        transformStack = new Stack<TransformInfo>();
        
        rules = new Dictionary<char, string>
        {

        };
        //transform.Rotate(Vector3.right * -90.0f);

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
        GenerateByParam(width, length, leafwidth, leaflength, angle, solidangle, iteration);
    }
    public void GenerateByParam(float width, float length, float leafwidth, float leaflength, float angle, float solidangle, int iteration)
    {
        CurrentString = Axiom;
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
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        StringBuilder sb = new StringBuilder();
        for(int i=0; i < iteration; i++)
        {
            foreach (char c in CurrentString)
            {
                sb.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());
                //Debug.Log(c);
                //Debug.Log(sb);
            }
            CurrentString = sb.ToString();
            //Debug.Log(CurrentString);
            //Debug.Log(i);
            sb = new StringBuilder ();
        }

        for (int i=0; i<CurrentString.Length; i++)
        {
             switch (CurrentString[i])
            {
                case 'F':
                    Vector3 initialPosition = transform.position;
                    transform.Translate(Vector3.up * length);
                    if (isLeaf(CurrentString, i))
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


    public void ResetTreeF()
    {
        IAxiom.text = "";
        IWidth.text = "";
        ILength.text = "";
        ILeafWidth.text = "";
        ILeafLength.text = "";
        IAngle.text = "";
        ISolidAngle.text = "";
        IIteration.text = "";
        CurrentString = Axiom;

        width = 0.0f;
        length = 0.0f;
        leafwidth = 0.0f;
        leaflength = 0.0f;
        angle = 0.0f;
        solidangle = 0.0f;
        iteration = 0;
}

    public void GenerateAxiomF()
    {
        Axiom = IAxiom.text;
        CurrentString = Axiom;
    }

    public void ResetAxiomF()
    {
        IAxiom.text = "";
        Axiom = null;
    }

    public void GenerateRulesF()
    {
        rules.Add(IRules_Key.text[0], IRules_Value.text);
        IRules_Key.text = "";
        IRules_Value.text = "";
    }

    public void ResetRulesF()
    {
        rules.Clear();
    }

    public void StepBackF()
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
        if (iteration <= 0)
        {
            return;
        }
        iteration--;
        IIteration.text = iteration.ToString();
        GenerateByParam(width, length, leafwidth, leaflength, angle, solidangle, iteration);
        Debug.Log("done!");
        Generate();
    }

    public void StepForwardF()
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
        Debug.Log("done!");
        iteration++;
        IIteration.text = iteration.ToString();
        GenerateByParam(width, length, leafwidth, leaflength, angle, solidangle, iteration);
        Generate();
    }

    public void Come2EightTreeF()
    {
        Destroy(Tree);
        rules.Clear();
        ResetTreeF();
        ResetAxiomF();
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
        SceneManager.LoadScene(1);
    }

    public void OnEnable()
    {
        GenerateTree.onClick.AddListener(Generate);
        ResetTree.onClick.AddListener(ResetTreeF);
        GenerateAxiom.onClick.AddListener(GenerateAxiomF);
        ResetAxiom.onClick.AddListener(ResetAxiomF);
        GenerateRules.onClick.AddListener(GenerateRulesF);
        ResetRules.onClick.AddListener(ResetRulesF);
        StepBack.onClick.AddListener(StepBackF);
        StepForward.onClick.AddListener(StepForwardF);
        Come2EightTree.onClick.AddListener(Come2EightTreeF);
}

}
