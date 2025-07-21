using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.TextCore;
using UnityEngine.TextCore.LowLevel;

public static class RuntimeSpriteAssetBuilder
{
    /// <summary>
    /// Builds and returns a TMP_SpriteAsset at runtime containing
    /// one sprite per ResourceDef, laid out in a single atlas.
    /// </summary>
    public static TMP_SpriteAsset BuildResourceSpriteAsset(int atlasSize = 1024, int cellSize = 128, int padding = 0)
    {
        // 1) Collect all sprites in DefDatabase order
        var defs = DefDatabase<ResourceDef>.AllDefs;
        var sprites = defs.Select(d => d.Sprite).ToArray();
        var smallTexes = ResizeSpritesIntoTextures(sprites, cellSize);

        // 2) Pack into a new atlas
        var atlasTex = new Texture2D(atlasSize, atlasSize, TextureFormat.RGBA32, false);
        var rects = atlasTex.PackTextures(smallTexes, padding);
        atlasTex.Apply();

        // 3) Create the TMP_SpriteAsset
        TMP_SpriteAsset spriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
        spriteAsset.name = "ResourceSpriteAsset";
        spriteAsset.spriteSheet = atlasTex;

        // 2) Assign material
        var spriteMat = new Material(Shader.Find("TextMeshPro/Sprite"));
        spriteMat.mainTexture = atlasTex;
        spriteMat.name = "ResourceSpriteAsset Material";
        spriteAsset.material = spriteMat;

        // Prepare tables
        spriteAsset.spriteInfoList = new List<TMP_Sprite>();
        spriteAsset.spriteGlyphTable.Clear();
        spriteAsset.spriteCharacterTable.Clear();

        for (int i = 0; i < sprites.Length; i++)
        {
            var def = defs[i];
            var sprite = sprites[i];
            var r = rects[i];

            // TMP_Sprite metadata
            TMP_Sprite info = new TMP_Sprite
            {
                name = def.DefName,
                hashCode = def.DefName.GetHashCode(),

                // position + size in pixels
                x = (short)(r.x * atlasTex.width),
                y = (short)(r.y * atlasTex.height),
                width = (short)(r.width * atlasTex.width),
                height = (short)(r.height * atlasTex.height),

                pivot = new Vector2(0.5f, 0.5f),
                xOffset = 0,
                yOffset = 90,
                xAdvance = cellSize,
            };
            spriteAsset.spriteInfoList.Add(info);

            // Build the GlyphRect from that info
            var glyphRect = new GlyphRect((int)info.x, (int)info.y, (int)info.width, (int)info.height);

            // Metrics just as before
            var metrics = new GlyphMetrics(
                info.width,
                info.height,
                info.xOffset,
                info.yOffset,
                info.xAdvance
            );

            // 6‑parameter ctor: (uint index, GlyphMetrics metrics, GlyphRect rect, float scale, int atlasIndex, Sprite sprite)
            var glyph = new TMP_SpriteGlyph(
                (uint)info.hashCode,
                metrics,
                glyphRect,
                1.5f,      // scale
                0,         // atlas index
                sprite     // reference back to the Sprite if you need it
            );
            spriteAsset.spriteGlyphTable.Add(glyph);

            // And the character entry
            var character = new TMP_SpriteCharacter(
                (uint)info.hashCode,
                glyph
            )
            { name = def.DefName };
            spriteAsset.spriteCharacterTable.Add(character);
        }

        // Build lookup tables
        spriteAsset.UpdateLookupTables();

        return spriteAsset;
    }

    private static Texture2D[] ResizeSpritesIntoTextures(Sprite[] sprites, int cellSize = 128)
    {
        var smalls = new List<Texture2D>(sprites.Length);
        foreach (var spr in sprites)
        {
            // render the source texture into a 128×128 RT
            var rt = RenderTexture.GetTemporary(cellSize, cellSize, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(spr.texture, rt);

            // read it back into a Texture2D
            var tex = new Texture2D(cellSize, cellSize, TextureFormat.RGBA32, false);
            var prev = RenderTexture.active;
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, cellSize, cellSize), 0, 0);
            tex.Apply();
            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);

            smalls.Add(tex);
        }
        return smalls.ToArray();
    }
}
