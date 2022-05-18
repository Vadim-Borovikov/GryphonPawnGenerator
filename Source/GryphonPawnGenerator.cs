using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Verse;

namespace GryphonPawnGenerator
{
    [UsedImplicitly]
    [StaticConstructorOnStartup]
    public static class GryphonPawnGenerator
    {
        static GryphonPawnGenerator() //our constructor
        {
            new Harmony("com.rimworld.mod.gryphon.pawn_generator").PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}