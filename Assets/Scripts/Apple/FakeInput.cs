using UnityEngine;

public class FakeInput : MonoBehaviour
{
    public FakeInput(float speed)
    {
        _speed = speed;
        _fakePoseInput = Vector3.zero;
    }

    public void Configure(float speed)
    {
        _speed = speed;
        _fakePoseInput = Vector3.zero;
    }

    [SerializeField] private float _speed;
    [SerializeField] private Vector3 _fakePoseInput = Vector3.zero;

    public Vector3 FakePose
    {
        get => _fakePoseInput;
        set { _fakePoseInput = value; }
    }

    public void Update()
    {
        _fakePoseInput += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * _speed * Time.deltaTime;
    }
}