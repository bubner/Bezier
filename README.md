# Bézier Curves
###### https://serve.bubner.me/unity/bezier
$$\text{Bézier curve general formula}$$
$$B(t) = \sum_{i=0}^{n} \binom{n}{i} (1-t)^{n-i} t^i P_i$$
$$\therefore B(t) = \sum_{i=0}^{n} \frac{n!}{i!(n-i)!} (1-t)^{n-i} t^i P_i$$
$$\because \binom{n}{k} = \frac{n!}{k!(n-k)!}$$
<br>
This small Unity app generates Bézier curves using the explicit definition of summated binomial coefficients and linear interpolation. Created as a proof-of-concept and for fun to learn more about the math involved in computer graphics.
## Features
- Add up to 171 points (`double` datatype precision)
- Remove individual or all points
- Edit points and reflect changes dynamically and instantly (like Desmos)
- Scrub along a vector by interpolation ratio `t` with a slider or animation
  - Draw-point scrubbing `t` shows construction lines as a set of linear interpolations
- Adjust resolution (delta t) of rendered curve
- Real-time curve rendering

[![bezier](https://github.com/bubner/Bezier/assets/81782264/2981d643-439e-44e1-8bf8-8032ffb1025a)](https://serve.bubner.me/unity/bezier)<br>
