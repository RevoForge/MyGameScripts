using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace Revo.Methods
{
    public static class SavedGameEncryption
    {
        private static readonly byte[] key = Encoding.UTF8.GetBytes("KeepingTheGoodGuysHonest");
        private static readonly byte[] iv = Encoding.UTF8.GetBytes("YouAreBadIfYouAreReadingThis");

        public static void EncryptAndSaveJson(string json, string filePath)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using MemoryStream msEncrypt = new();
            using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (StreamWriter swEncrypt = new(csEncrypt))
            {
                swEncrypt.Write(json);
            }
            File.WriteAllBytes(filePath, msEncrypt.ToArray());
        }

        public static string DecryptAndReadJson(string filePath)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            byte[] encryptedData = File.ReadAllBytes(filePath);

            using MemoryStream msDecrypt = new(encryptedData);
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class FloatRangeAttribute : Attribute
    {
        public float Min { get; }
        public float Max { get; }

        public FloatRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }

    public static class PlayerDetection
    {
        public static GameObject FindChildWithTag(Transform parent, string tag)
        {
            foreach (Transform child in parent)
            {
                if (child.CompareTag(tag))
                {
                    return child.gameObject;
                }
                GameObject foundChild = FindChildWithTag(child, tag);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }
            return null;
        }
    }

    public static class PositionCalculator
    {
        public static Vector3 CalculateObjectVelocity(Transform objectTransform, Queue<Vector3> positions, int bufferSize)
        {
            positions.Enqueue(objectTransform.position);

            if (positions.Count > bufferSize)
            {
                positions.Dequeue();
            }

            if (positions.Count == bufferSize)
            {
                Vector3 firstPosition = positions.Peek();
                Vector3 lastPosition = positions.ToArray()[bufferSize - 1];
                return (lastPosition - firstPosition);
            }

            return Vector3.zero;
        }
    }

    public static class ObjectInteraction
    {
        public static void PickupPhysicsObject(GameObject player, Transform itemHolder, Transform selfTransform, HingeJoint handJoint, Rigidbody selfRB, bool isWeapon, PickupAttackMonster weapon)
        {
            selfTransform.SetParent(itemHolder);
            selfRB.isKinematic = true;
            selfTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            handJoint.connectedBody = itemHolder.GetComponent<Rigidbody>();
            selfRB.isKinematic = false;

            if (isWeapon)
            {
                weapon.PlayerPickup(player);
            }
        }
        public static void ReleasePhysicsObject(Transform originalParent, GameObject itemHolder, Transform selfTransform, HingeJoint handJoint, Rigidbody selfRB, float thrownMultiplier, Vector3 objectVelocity)
        {
            originalParent.SetPositionAndRotation(itemHolder.transform.position, itemHolder.transform.rotation);
            selfTransform.SetParent(originalParent);
            handJoint.connectedBody = originalParent.GetComponent<Rigidbody>();
            selfRB.isKinematic = true;
            selfTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            selfRB.isKinematic = false;
            selfRB.velocity = itemHolder.GetComponent<Rigidbody>().velocity;
            selfRB.AddForce(objectVelocity * thrownMultiplier, ForceMode.VelocityChange);
        }
        public static void PickupObject(GameObject player, Transform Hand, Transform transform, Rigidbody rb, bool isWeapon, bool isMagic, PickupAttackMonster weapon, WandAttackMonster magic, PickupObjectScript objectScript)
        {
            transform.SetParent(Hand);
            Vector3 initialPosition = transform.localPosition;
            Quaternion initialRotation = transform.localRotation;
            rb.isKinematic = true;
            objectScript.StartMoveToHand(initialPosition, initialRotation);

            if (isWeapon)
            {
                weapon.PlayerPickup(player);
            }
            if (isMagic)
            {
                magic.PlayerPickup(player);
            }
        }
        public static void ReleaseObject(bool isMagic, Rigidbody selfRB, WandAttackMonster magic, Transform transform, Vector3 objectVelocity, float thrownMultiplier)
        {
            selfRB.isKinematic = false;

            if (isMagic)
            {
                magic.PlayerDrop();
            }
            transform.SetParent(null);
            selfRB.AddForce(objectVelocity * thrownMultiplier, ForceMode.VelocityChange);
        }
        public enum DamageType
        {
            Blunt = 0,
            Slashing = 1,
            Fire = 2,
            Acid = 3,
            Lightning = 4,
            Poison = 5,
            Magic = 6,
            Light = 7,
            Gravity = 8,
            Water = 9,
            Earth = 10,
            None = 99
        }
        public enum AttackZone
        {
            Front,
            Left,
            Right,
            Back,
            Center
        }
        public enum InventorySlots
        {
            TopLeft,
            TopCenter,
            TopRight,
            BottomLeft,
            BottomCenter,
            BottomRight
        }
        public enum WeaponTypes
        {
            None,
            AcidWand,
            FireWand,
            SwordSmall,
            SwordBig,
            Hammer,
            SwordADevil,
            SwordBDevil,
            Axe,
            AcidWandLoot,
            FireWandLoot,
            SwordSmallLoot,
            SwordBigLoot,
            HammerLoot,
            SwordADevilLoot,
            SwordBDevilLoot,
            AxeLoot,
            GoblinSword,
            GoblinDagger,
        }
        public static Dictionary<WeaponTypes, string> EnumToStringMapping = new()
    {
    { WeaponTypes.None, "None" },
    { WeaponTypes.AcidWand, "WandA1" },
    { WeaponTypes.FireWand, "WandF1" },
    { WeaponTypes.SwordSmall, "DaggerT1" },
    { WeaponTypes.SwordBig, "SwordT1" },
    { WeaponTypes.Hammer, "HammerT1" },
    { WeaponTypes.SwordADevil, "SwordD1" },
    { WeaponTypes.SwordBDevil, "SwordD1" },
    { WeaponTypes.Axe, "AxeT1" },
    { WeaponTypes.AcidWandLoot, "WandA2" },
    { WeaponTypes.FireWandLoot, "WandF2" },
    { WeaponTypes.SwordSmallLoot, "DaggerT2" },
    { WeaponTypes.SwordBigLoot, "SwordT2" },
    { WeaponTypes.HammerLoot, "HammerT2" },
    { WeaponTypes.SwordADevilLoot, "SwordD2" },
    { WeaponTypes.SwordBDevilLoot, "SwordD2" },
    { WeaponTypes.AxeLoot, "AxeT2" },
    { WeaponTypes.GoblinSword, "SwordT1" },
    { WeaponTypes.GoblinDagger, "DaggerT1" }

};
        public struct SpeedModifier
        {
            public float moveSpeedModifier;
            public float timeModifier;
            public SpeedModifier(float modifier1, float modifier2)
            {
                moveSpeedModifier = modifier1;
                timeModifier = modifier2;
            }
        }
        public static Dictionary<string, SpeedModifier> ParticleModifiers = new()
    {
        { "Spider Particles", new SpeedModifier(0.25f, 1.5f) },
        { "Acid Particles", new SpeedModifier(0.5f, 0.5f) },
    };
        public static string GetStringForWeaponType(WeaponTypes weaponType)
        {
            if (EnumToStringMapping.ContainsKey(weaponType))
            {
                return EnumToStringMapping[weaponType];
            }
            else
            {
                return "UnknownString";
            }
        }
        public interface IMonsterAI
        {
            int Damage { get; set; }
            bool ShowHealthBar { get; set; }
        }
        public interface IPlayerDetectionAI
        {
            IEnumerator PlayerDetected(GameObject playerCamera);
            void PlayerLost();
            void PlayerAttacked(GameObject player);
            void AttackZoneDetection(AttackZone zone);
        }
        [CreateAssetMenu(fileName = "PreFabWeapon", menuName = "PreFabWeapon")]
        public class PreFabWeapon : ScriptableObject
        {
            public GameObject prefab;
        }

        public static Vector3 CalculateRandomPosition(Transform transform, float dist, float minDist)
        {
            Vector3 randDir;
            NavMeshHit hit;
            do
            {
                randDir = transform.position + UnityEngine.Random.insideUnitSphere * dist;
            }
            while (!NavMesh.SamplePosition(randDir, out hit, dist, NavMesh.AllAreas) || Vector3.Distance(transform.position, hit.position) < minDist);
            return hit.position;
        }
    }

    public static class LootSystem
    {
        [Serializable]
        public class LootEntry
        {
            public GameObject lootObject;
            public float weight;
        }

        public static GameObject DropLoot(LootEntry[] lootEntries)
        {
            float totalWeight = 0f;

            // Calculate the total weight of all loot entries
            foreach (LootEntry entry in lootEntries)
            {
                totalWeight += entry.weight;
            }

            // Generate a random number within the total weight range
            float randomWeight = UnityEngine.Random.Range(0f, totalWeight);

            float cumulativeWeight = 0f;
            GameObject selectedObject = null;

            // Iterate through the loot entries and select one based on the weights
            foreach (LootEntry entry in lootEntries)
            {
                cumulativeWeight += entry.weight;

                // If the random weight is within the cumulative weight, select this entry's loot object
                if (randomWeight <= cumulativeWeight)
                {
                    selectedObject = entry.lootObject;
                    break;
                }
            }
            if (selectedObject != null)
            {
                return selectedObject;
            }
            return null;
        }
    }

    public static class SceneController
    {
        public static void LoadLevelScene(SettingsLoader settings, GameObject buttons, GameObject loadingBar)
        {
            settings.SaveChangedSettings(0);
            buttons.SetActive(false);
            loadingBar.SetActive(true);
        }
        [Serializable]
        public class SceneConfiguration
        {
            public string rightItemPrefabName;
            public string leftItemPrefabName;
            public string topLeftItem;
            public string bottomLeftItem;
            public string topCenterItem;
            public string bottomCenterItem;
            public string topRightItem;
            public string bottomRightItem;
            public string topLeftItemName;
            public string bottomLeftItemName;
            public string topCenterItemName;
            public string bottomCenterItemName;
            public string topRightItemName;
            public string bottomRightItemName;

        }
    }
    public static class SoundController
    {
        [Serializable]
        public class NamedAudioClip
        {
            public string name;
            public AudioClip clip;
        }
    }

    public class CharacterControllerDodging
    {
        public static IEnumerator PerformDodgeMovement(NavMeshAgent controller, Vector3 dodgeMovement, float duration, Transform mobPosition)
        {
            {
                float elapsedTime = 0.0f;
                Vector3 startPosition = mobPosition.position;
                Vector3 targetPosition = startPosition + dodgeMovement;

                while (elapsedTime < duration)
                {
                    float t = elapsedTime / duration;
                    Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, t);
                    Vector3 movementDelta = newPosition - mobPosition.position;

                    controller.Move(movementDelta);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                controller.Move(targetPosition - controller.transform.position);
            }
        }
    }
    public class StatSystem
    {

        public static int CalculateStat(int stat, Dictionary<int, int> multipliers)
        {
            int result = 0;
            int remainingStat = stat;

            foreach (var entry in multipliers)
            {
                int range = entry.Key;
                int multiplier = entry.Value;

                if (remainingStat <= 0)
                {
                    break;
                }

                int statContribution = Math.Min(remainingStat, range) * multiplier;
                result += statContribution;
                remainingStat -= range;
            }
            return result;
        }
    }
}
