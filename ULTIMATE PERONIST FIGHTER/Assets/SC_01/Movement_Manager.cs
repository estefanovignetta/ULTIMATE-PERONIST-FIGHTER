using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Movement_Manager : MonoBehaviour {
    public Camera cam;
    private Animator anim;
    private BoxCollider bc;
    private Sound_Manager sm;
    public AudioClip _punch01, _punch02, _throw;

    private Vector3 charscreenpos;
    public bool facing_right, facing_up;
    public bool moving_right, moving_left, moving_up, moving_down;
    public float speed;

    private float h, v;
    public int onAttack, attack_counter;
    private int onBeingHit;

    public List<ColliderRestrictions> MovementRestrictionList = new List<ColliderRestrictions>();
    private bool lockdown;
    private Game_Manager gm;
    
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    // Use this for initialization
    void Awake()
    {
        gm = GameObject.Find("Game Manager").GetComponent<Game_Manager>();
        anim = GetComponent<Animator>();
        sm = GetComponent<Sound_Manager>();
        bc = GetComponent<BoxCollider>();
        onAttack = onBeingHit= 0;
        attack_counter = 0;
        facing_right = facing_up = true;
        moving_right = moving_left = moving_up = moving_down= true;
        lockdown = false;
    }
/* -------------------------------------------------------------------------------------------------------------------------------- */
    // Update is called once per frame
    void Update()
    {
        anim.SetBool("Walking", false);

        //CHECKING IF CHARACTER IS PUNCHING SOMETHING OR BEING PUNCHED
        if (onAttack > 0 || onBeingHit > 0) return;
        charscreenpos = cam.WorldToViewportPoint(transform.position);

        //CHECKING HORIZONTAL MOVEMENT
        h = Input.GetAxisRaw("LeftJoystickHorizontal");
        if ((h > 0 && !facing_right) || (h < 0 && facing_right)) Flip();
        if ((h > 0 && (charscreenpos.x >= 0.95 || !moving_right)) || (h < 0 && (charscreenpos.x <= 0.05 || !moving_left)))
                h = 0;

        //CHECKING VERTICAL MOVEMENT
        v = Input.GetAxisRaw("LeftJoystickVertical");
        if ((v > 0 && !facing_up) || (v < 0 && facing_up)) facing_up = !facing_up;
        //if ((v > 0 && (charscreenpos.y >= 0.3 || !moving_up)) || (v < 0 && (charscreenpos.y <= 0 || !moving_down)))
        if ((v > 0 && !moving_up) || (v < 0 && (charscreenpos.y <= 0 || !moving_down)))
            v = 0;

        //CHECKING BUTTONS
        if (Input.GetButtonDown("XButton"))
        {
            h = v = 0;
            anim.SetTrigger("Attack_01");
            //sm.SendMessage("PlayOutLoud_throw");
            sm.SendMessage("PlayOutLoud", _throw);
        }

        if (Input.GetButtonDown("YButton"))
        {
            h = v = 0;
            anim.SetTrigger("Attack_02");
            //sm.SendMessage("PlayOutLoud_throw");
            sm.SendMessage("PlayOutLoud", _throw);
        }

        //EXECUTING MOVEMENTS
        if (h != 0 || v != 0)
        {
            transform.Translate(h * Time.deltaTime * speed, v * Time.deltaTime * speed, v * Time.deltaTime * speed);
            anim.SetBool("Walking", true);
        }

        //ADJUSTING CAMERA
        if (charscreenpos.x > 0.7 && h > 0 && !lockdown)
            cam.transform.Translate(h * Time.deltaTime * speed,0,0);
    }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    public void setOnAttack(int state)
    {
        onAttack = state;
        return;
    }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    public void setAttackCounter(int state)
    {
        attack_counter = state;
        return;
    }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    public void setOnBeingHit(int state)
    {
        onBeingHit = state;
        onAttack = 0;
        attack_counter = 0;
        return;
    }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    protected void Flip()
    {
        facing_right = !facing_right;
        // Multiply the player's x local scale by -1
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    void OnTriggerEnter(Collider myCollider)
    {
        bool this_movingright, this_movingleft, this_movingup, this_movingdown;
        this_movingright = this_movingleft = this_movingup = this_movingdown = true;

        //CALCULATING THE VERTICES OF THE COLLIDERS
        float upper_ref = bc.center.y + bc.size.y / 2,
            left_ref = bc.center.x - bc.size.x / 2,
            right_ref = bc.center.x + bc.size.x / 2,
            front_ref = bc.center.z - bc.size.z / 2,
            back_ref = bc.center.z + bc.size.z / 2;
        Vector3 my_point1 = transform.TransformPoint(new Vector3(left_ref, upper_ref, front_ref)),
            my_point2 = transform.TransformPoint(new Vector3(right_ref, upper_ref, front_ref)),
            my_point3 = transform.TransformPoint(new Vector3(left_ref, upper_ref, back_ref)),
            my_point4 = transform.TransformPoint(new Vector3(right_ref, upper_ref, back_ref));

        if (my_point1.x >= my_point2.x)
        {
            float aux = my_point1.x;
            my_point1.x = my_point3.x = my_point2.x;
            my_point2.x = my_point4.x = aux;
        }

        BoxCollider their_bc = myCollider.GetComponent<BoxCollider>();
        upper_ref = their_bc.center.y + their_bc.size.y / 2;
        left_ref = their_bc.center.x - their_bc.size.x / 2;
        right_ref = their_bc.center.x + their_bc.size.x / 2;
        front_ref = their_bc.center.z - their_bc.size.z / 2;
        back_ref = their_bc.center.z + their_bc.size.z / 2;
        Vector3 their_point1 = myCollider.transform.TransformPoint(new Vector3(left_ref, upper_ref, front_ref)),
            their_point2 = myCollider.transform.TransformPoint(new Vector3(right_ref, upper_ref, front_ref)),
            their_point3 = myCollider.transform.TransformPoint(new Vector3(left_ref, upper_ref, back_ref)),
            their_point4 = myCollider.transform.TransformPoint(new Vector3(right_ref, upper_ref, back_ref));

        if (their_point1.x >= their_point2.x)
        {
            float aux = their_point1.x;
            their_point1.x = their_point3.x = their_point2.x;
            their_point2.x = their_point4.x = aux;
        }

        bool case1, case2, case3, case4;
        case1 = case2 = case3 = case4 = false;
        //FINDING CASE OF COLLISION
        //CASE 1 - target upper side collision
        if (my_point1.z <= their_point3.z && my_point3.z >= their_point3.z)
            case1 = true;
        //CASE 2 - target right side collision
        if (my_point1.x <= their_point2.x && my_point2.x >= their_point2.x)
            case2 = true;
        //CASE 3 - target lower side collision
        if (my_point3.z >= their_point1.z && my_point1.z <= their_point1.z)
            case3 = true;
        //CASE 4 - target left side collision
        if (my_point2.x >= their_point1.x && my_point1.x <= their_point1.x)
            case4 = true;

        float dist_x, dist_z;
        //CONSTRAINING MOVEMENT
        if (case1 && case4)
        {
            dist_z = their_point3.z - my_point2.z;
            dist_x = my_point2.x - their_point3.x;
            if (dist_z > dist_x) this_movingright = false;
            else this_movingdown = false;
        }
        else if (case1 && case2)
        {
            dist_z = their_point4.z - my_point1.z;
            dist_x = their_point4.x - my_point1.x;
            if (dist_z > dist_x) this_movingleft = false;
            else this_movingdown = false;
        }
        else if (case2 && case3)
        {
            dist_z = my_point3.z - their_point2.z;
            dist_x = their_point2.x - my_point3.x;
            if (dist_z > dist_x) this_movingleft = false;
            else this_movingup = false;
        }
        else if (case3 && case4)
        {
            dist_z = my_point4.z - their_point1.z;
            dist_x = my_point4.x - their_point1.x;
            if (dist_z > dist_x) this_movingright = false;
            else this_movingup = false;
        }
        else if (case1) this_movingdown = false;
        else if (case2) this_movingleft = false;
        else if (case3) this_movingup = false;
        else if (case4) this_movingright = false;

        //SAVING THE COLLIDER AND FORBIDDEN DIRECTION
        ColliderRestrictions cr = new ColliderRestrictions();
        cr.col = myCollider;
        cr.moving_right = this_movingright;
        cr.moving_left = this_movingleft;
        cr.moving_up = this_movingup;
        cr.moving_down = this_movingdown;
        MovementRestrictionList.Add(cr);

        //APPLYING THE NEW RESTRICTION
        if (!this_movingright) moving_right = false;
        else if (!this_movingleft) moving_left = false;
        else if (!this_movingup) moving_up = false;
        else if (!this_movingdown) moving_down = false;
    }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    void OnTriggerStay(Collider myCollider)
    {
        //CHECKING IF CHARACTER IS ATTACKING EFFECTIVELY
        if (onAttack > 0 && myCollider.CompareTag("Enemy") &&
            ((facing_right && myCollider.transform.position.x > transform.position.x) || (!facing_right && myCollider.transform.position.x < transform.position.x))
            && attack_counter > 0)
        {
            myCollider.SendMessage("ApplyDamage", 1);
            attack_counter = 0;
            if (onAttack == 1) sm.SendMessage("PlayOutLoud", _punch01);
            else if (onAttack == 2) sm.SendMessage("PlayOutLoud", _punch02);
        }
    }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    void OnTriggerExit(Collider myCollider)
    {
        int i;
        for (i = 0; i < MovementRestrictionList.Count; i++)
        {
            if (MovementRestrictionList[i].col == myCollider) break;
        }
        if (!MovementRestrictionList[i].moving_right) moving_right = true;
        else if (!MovementRestrictionList[i].moving_left) moving_left = true;
        else if (!MovementRestrictionList[i].moving_up) moving_up = true;
        else if (!MovementRestrictionList[i].moving_down) moving_down = true;
        MovementRestrictionList.RemoveAt(i);

        //APPLYING OTHER RESTRICTIONS
        for (i = 0; i < MovementRestrictionList.Count; i++)
        {
            if (!MovementRestrictionList[i].moving_right) moving_right = false;
            else if (!MovementRestrictionList[i].moving_left) moving_left = false;
            else if (!MovementRestrictionList[i].moving_up) moving_up = false;
            else if (!MovementRestrictionList[i].moving_down) moving_down = false;
        }
    }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    public struct ColliderRestrictions
    {
        public Collider col;
        public bool moving_right, moving_left, moving_up, moving_down;
    }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    public void setLockdown(bool state)
    {
        lockdown = state;
        return;
    }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    void OnDestroy()
    {
        if (gm != null) gm.SendMessage("SwitchMenu");
    }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
}