// WARNING
// This file has been generated automatically by MonoDevelop to
// mirror C# types. Changes in this file made by drag-connecting
// from the UI designer will be synchronized back to C#, but
// more complex manual changes may not transfer correctly.


#import <UIKit/UIKit.h>
#import <Foundation/Foundation.h>
#import <CoreGraphics/CoreGraphics.h>


@interface SmsComposerViewController : UIViewController {
	UILabel *_LabelComposeMessage;
	UITextView *_Message;
	UILabel *_LabelUseMagicExpression;
	UILabel *_CharacterCount;
	UILabel *_LabelMaxContact;
	UIButton *_MaskButton;
	UIButton *_SendButton;
	UISlider *_Slider;
	UILabel *_SliderCount;
	UISwitch *_MagicExpressionSwitch;
}

@property (nonatomic, retain) IBOutlet UILabel *LabelComposeMessage;

@property (nonatomic, retain) IBOutlet UITextView *Message;

@property (nonatomic, retain) IBOutlet UILabel *LabelUseMagicExpression;

@property (nonatomic, retain) IBOutlet UILabel *CharacterCount;

@property (nonatomic, retain) IBOutlet UILabel *LabelMaxContact;

@property (nonatomic, retain) IBOutlet UIButton *MaskButton;

@property (nonatomic, retain) IBOutlet UIButton *SendButton;

@property (nonatomic, retain) IBOutlet UISlider *Slider;

@property (nonatomic, retain) IBOutlet UILabel *SliderCount;

@property (nonatomic, retain) IBOutlet UISwitch *MagicExpressionSwitch;

- (IBAction)SendButtonTouchedUp:(id)sender;

- (IBAction)MagicExpressionValueChanged:(id)sender;

- (IBAction)MaxRecipientInfoTouchUpInside:(id)sender;

- (IBAction)MagicExpressionInfoTouchUpInside:(id)sender;

@end
