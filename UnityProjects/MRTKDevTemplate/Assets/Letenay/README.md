# Prototypy v tejto diplomovej práci

Priečinok `Assets/Letenay` obsahuje prototypy vytvorené v rámci mojej diplomovej práce.

## Hlavné prístupy ovládania
Priečinok obsahuje 3 hlavné prístupy:
* **Šípky**
* **Joystick**
* **OneAxis**

Každý z týchto priečinkov má štandardnú štruktúru Unity projektu (`Materials`, `Prefabs`, `Scenes`, `Scripts`...).


> Scény v tomto priečinku sú testovacie a neboli použité pri finálnej evaluácii.
> Scény použité na evaluáciu sa nachádzajú v priečinku `Assets/iChair`.

## Pomocné funkcie (Utils)
Priečinok `Utils` obsahuje pomocný skripty, spoločný pre viaceré prototypy:
* Bluetooth komunikácia s vozíkom.
* Kontrola stavu sledovania očí.
* Iné pomocné funkcie.

---

## Simulácia pohľadu

### Simulácia bez hardvéru
Všetky prototypy majú predvolene zapnutú simuláciu. Pohľad je v scéne znázornený **bielym trojuholníkom**.
* **Simulácia pohľadu:** Držte kláves `TAB` a pohybujte myšou.
* **Debug info:** Pri simulácii sa v scéne zobrazuje text `x:{x}, y:{y}`, ktorý reprezentuje hodnoty odosielané do vozíka.
    * Hodnoty sú v rozsahu **0 – 255**.
    * Neutrálne hodnoty **(128, 128)** znamenajú, že vozík stojí.

### Použitie eye trackera Neon
Pre použitie eye trackera Neon postupujte nasledovne:

1. Zapojte eye tracker **Neon** do zariadenia **Neon Companion** (telefón).
2. Pripojte telefón na **rovnakú Wi-Fi**, na ktorej je pripojený počítač/headset.
3. V scéne, ktorú chcete spustiť, nájdite objekt `MRTK NeonXR Variant`.
4.  V hierarchii objektu `MRTK NeonXR Variant` nájdite objekt `PupilLabs`  
5. Vypnite možnosť `Simulation Enabled`.