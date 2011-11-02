//
//  LinkView.h
//  BDViewer
//
//  Created by Liz Dovey on 11-11-01.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface LinkedNoteView : UIViewController <UIWebViewDelegate>


@property (retain, nonatomic) NSString *linkedNoteId;
@property (retain, nonatomic) NSString *detailHTMLString;

@property (retain, nonatomic) IBOutlet UIWebView *dataWebView;

-(id)initWithLinkId:(NSString *)pLinkId;

@end
