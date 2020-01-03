# FacePush
[![License](https://poser.pugx.org/padraic/humbug_get_contents/license)](https://opensource.org/licenses/BSD-3-Clause)

## Synopsis
This paper presents FacePush, a Head-Mounted Display (HMD) integrated with a pulley system to generate normal forces on a user's face in virtual reality (VR). The mechanism of FacePush is obtained by shifting torques provided by two motors that press upon a user's face via utilization of a pulley system. FacePush can generate normal forces of varying strengths and apply those to the surface of the face. To inform our design of FacePush for noticeable and discernible normal forces in VR applications, we conducted two studies to iden- tify the absolute detection threshold and the discrimination threshold for users' perception. After further consideration in regard to user comfort, we determined that two levels of force, 2.7 kPa and 3.375 kPa, are ideal for the development of the FacePush experience via implementation with three applications which demonstrate use of discrete and continuous normal force for the actions of boxing, diving, and 360 guidance in virtual reality. In addition, with regards to a virtual boxing application, we conducted a user study evaluating the user experience in terms of enjoyment and realism and collected the user's feedback.

## Code Need

Unity 5.6.3 and Arduino

## API Reference

### Arduino
* [Arduino I2C Tutorial](https://arduino169.blogspot.tw/2015/07/arduino-i2c.html?m=1)
* [Tutorial for Monster Motor Shield VNH2SP30 ](http://www.instructables.com/id/Monster-Motor-Shield-VNH2SP30/)
* [Arduino Interrupt ](https://chtseng.wordpress.com/2015/12/25/arduino-%E4%B8%AD%E6%96%B7%E5%8A%9F%E8%83%BD/)
* [PID – Introduction](http://brettbeauregard.com/blog/2011/04/improving-the-beginners-pid-introduction/)
* [Arduino-PID-Library](https://github.com/br3ttb/Arduino-PID-Library)
* [ArduinoThread](https://github.com/ivanseidel/ArduinoThread)
* [PinChangeInt](https://github.com/GreyGnome/PinChangeInt)

### unity package
* [Hurricane Wind FX ](https://assetstore.unity.com/packages/vfx/particles/environment/hurricane-wind-fx-104948)
* [Boxer Animations ](https://assetstore.unity.com/packages/3d/animations/boxer-animations-96950)
* [White Swimmer ](https://assetstore.unity.com/packages/3d/white-swimmer-10686-tris-39121)
* [Underwater FX ](https://assetstore.unity.com/packages/vfx/particles/environment/underwater-fx-61157)

### Motor
* [Metal Gearmotor](https://www.robotshop.com/en/12v-170rpm-econ-metal-gearmotor.html#Specifications)

## License

Available under [the BSD-3-Clause license](https://opensource.org/licenses/BSD-3-Clause).

