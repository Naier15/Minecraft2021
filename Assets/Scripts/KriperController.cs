using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KriperController : MonoBehaviour
{
    public float jumpSpeed;
    public float speedy;
    public float gravity;

    public Material red_material;
    public Material kriper_material;

    protected float temp_x;
    protected float temp_z;
    protected int i = 0;
    private Vector3 moveDirection = Vector3.zero;
    protected int k = 0;
    protected Vector3 nearby;

    //public GameObject krip;
    //public List<GameObject> krips = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }


    IEnumerator Redding(CharacterController controller)
    {
        controller.GetComponent<MeshRenderer>().material = red_material;
        yield return new WaitForSeconds(1);
        controller.GetComponent<MeshRenderer>().material = kriper_material;
        yield return new WaitForSeconds(1);
        controller.GetComponent<MeshRenderer>().material = red_material;
        yield return new WaitForSeconds(1);
        Vector3 temp_pos = controller.transform.position;

        controller.gameObject.SetActive(false);

        var wg = GameObject.Find("WorldGenerator").GetComponent<WorldGenerator>();
        int i = 0;
        List<GameObject> nums = new List<GameObject>();
        foreach (var elem in wg.all_blocks)
        {
            if (elem != null)
            {
                if (Vector3.Distance(temp_pos, elem.transform.position) <= 5 && elem.name != "Badrock(Clone)")
                {
                    Destroy(wg.all_blocks[i]);
                    nums.Add(wg.all_blocks[i]);
                }
            }
            ++i;
        }
        foreach (GameObject k in nums)
            wg.all_blocks.Remove(k);
    }


    // Update is called once per frame
    void Update()
    {
        var me = GameObject.Find("Capsule");
        transform.LookAt(me.transform);

        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(0, 0, 1);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speedy;
            

            Vector3 border = controller.gameObject.transform.position; border.x--;
            Vector3 border2 = controller.gameObject.gameObject.transform.position; border2.x++;
            Vector3 border3 = controller.gameObject.transform.position; border3.z--;
            Vector3 border4 = controller.gameObject.transform.position; border4.z++;
            if (Physics.Linecast(controller.gameObject.transform.position, border) || Physics.Linecast(controller.gameObject.transform.position, border2)
                || Physics.Linecast(controller.gameObject.transform.position, border3) || Physics.Linecast(controller.gameObject.transform.position, border4))
                moveDirection.y = jumpSpeed;
        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);


        nearby = controller.transform.position - me.transform.position;
            if (nearby.x <= 3 && nearby.x >= -3 && nearby.y <= 3 && nearby.y >= -3 && nearby.z <= 3 && nearby.z >= -3)
                StartCoroutine(Redding(controller));
    }
}
