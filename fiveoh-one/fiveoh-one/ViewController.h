//
//  ViewController.h
//  fiveoh-one
//
//  Created by Karl Schulze on 2011-08-15.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface ViewController : UIViewController <UITableViewDelegate>
{
    UITableView *mainTableView;
}

@property (strong, nonatomic) IBOutlet UITableView      *mainTableView;

@end
