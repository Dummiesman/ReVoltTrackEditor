void MMPassOneClip(float alphaClipThreshold, fixed4 color)
{
    clip(color.a - alphaClipThreshold);
}

void MMPassTwoClip(float alphaClipThreshold, fixed4 color) 
{
    clip(alphaClipThreshold - color.a);
    clip(color.a - 0.1f);
}