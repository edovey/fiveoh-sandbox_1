//
//  ChapterListView.h
//  BDViewer
//
//  Created by Liz Dovey on 11-10-19.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface ChapterListView : UIViewController <UITableViewDelegate, UITableViewDataSource>


@property (retain, nonatomic) IBOutlet UITableView *dataTableView;

@property (retain, nonatomic) NSArray *chapterArray;
@end
