using UnityEngine;
using UnityEngine.UI;
using System;

public enum Edition
{
	Regular,
	Polychrome,
	Foil,
	Negative
}

[RequireComponent(typeof(Image))]
public class ShaderCode : MonoBehaviour
{
    Image image;
	Material material;
	public Edition edition;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        material = new Material(image.material);
        image.material = material;

        string[] editions = new string[4];
        editions[0] = "REGULAR";
        editions[1] = "POLYCHROME";
        editions[2] = "FOIL";
        editions[3] = "NEGATIVE";

		for (int i = 0; i < image.material.enabledKeywords.Length; i++)
		{
			image.material.DisableKeyword(image.material.enabledKeywords[i]);
		}

		Array values = Enum.GetValues(typeof(Edition));
		int randomIndex = UnityEngine.Random.Range(0, values.Length);
		edition = (Edition)values.GetValue(randomIndex);
        image.material.EnableKeyword("_EDITION_" + edition.ToString().ToUpper());
    }

    // Update is called once per frame
    void Update()
    {
        // Get the current rotation as a quaternion
        Quaternion currentRotation = transform.parent.localRotation;

        // Convert the quaternion to Euler angles
        Vector3 eulerAngles = currentRotation.eulerAngles;

        // Get the X-axis angle
        float xAngle = eulerAngles.x;
        float yAngle = eulerAngles.y;

        // Ensure the X-axis angle stays within the range of -90 to 90 degrees
        xAngle = ClampAngle(xAngle, -90f, 90f);
        yAngle = ClampAngle(yAngle, -90f, 90f);

        material.SetVector("_Rotation", new Vector2(ExtensionMethods.Remap(xAngle, -20f, 20f, -.5f, .5f), ExtensionMethods.Remap(yAngle, -20f, 20f, -.5f, .5f)));
    }

    // Method to clamp an angle between a minimum and maximum value
    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -180f)
            angle += 360f;
		if (angle > 180f)
			angle -= 360f;
			
        return Mathf.Clamp(angle, min, max);
    }
}
