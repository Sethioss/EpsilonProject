#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 3.0.12
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------


public class AkAcousticSurface : global::System.IDisposable {
  private global::System.IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal AkAcousticSurface(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = cPtr;
  }

  internal static global::System.IntPtr getCPtr(AkAcousticSurface obj) {
    return (obj == null) ? global::System.IntPtr.Zero : obj.swigCPtr;
  }

  internal virtual void setCPtr(global::System.IntPtr cPtr) {
    Dispose();
    swigCPtr = cPtr;
  }

  ~AkAcousticSurface() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          AkSoundEnginePINVOKE.CSharp_delete_AkAcousticSurface(swigCPtr);
        }
        swigCPtr = global::System.IntPtr.Zero;
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public AkAcousticSurface() : this(AkSoundEnginePINVOKE.CSharp_new_AkAcousticSurface(), true) {
  }

  public uint textureID { set { AkSoundEnginePINVOKE.CSharp_AkAcousticSurface_textureID_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkAcousticSurface_textureID_get(swigCPtr); } 
  }

  public float transmissionLoss { set { AkSoundEnginePINVOKE.CSharp_AkAcousticSurface_transmissionLoss_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkAcousticSurface_transmissionLoss_get(swigCPtr); } 
  }

  public string strName { set { AkSoundEnginePINVOKE.CSharp_AkAcousticSurface_strName_set(swigCPtr, value); }  get { return AkSoundEngine.StringFromIntPtrString(AkSoundEnginePINVOKE.CSharp_AkAcousticSurface_strName_get(swigCPtr)); } 
  }

  public void Clear() { AkSoundEnginePINVOKE.CSharp_AkAcousticSurface_Clear(swigCPtr); }

  public void DeleteName() { AkSoundEnginePINVOKE.CSharp_AkAcousticSurface_DeleteName(swigCPtr); }

  public static int GetSizeOf() { return AkSoundEnginePINVOKE.CSharp_AkAcousticSurface_GetSizeOf(); }

  public void Clone(AkAcousticSurface other) { AkSoundEnginePINVOKE.CSharp_AkAcousticSurface_Clone(swigCPtr, AkAcousticSurface.getCPtr(other)); }

}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.